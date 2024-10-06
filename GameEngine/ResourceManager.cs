using System.Net.Mime;
using System.Numerics;
using GameEngine.WebGPU;
using ObjLoader.Loader.Loaders;

namespace GameEngine;

public class ResourceManager
{
    public static HttpClient HttpClient = new();

    public static async Task<string> LoadString(string name)
    {
        var stream = await LoadStream(name);
        var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<Stream> LoadStream(string name)
    {
        string baseUrl;
#if DEBUG
        baseUrl = "https://localhost:7030/";
#else
        baseUrl = "https://flurinbruehwiler.github.io/WebGPUGameEngine/";
#endif

        return await HttpClient.GetStreamAsync(baseUrl + name);
    }

    private static IObjLoader? _objLoader;

    public static async Task<Texture> LoadTexture(string name)
    {
        var stream = await LoadStream(name);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var imageBitmap = await JsWindow.CreateImageBitmap(ms.ToArray(), new BitmapOptions
        {
            ClorSpaceConversion = "none"
        });
        var gpuTexture = Renderer.CreateTextureFromBitmap(imageBitmap);
        return new Texture
        {
            GpuTexture = gpuTexture
        };
    }

    public static async Task<Model> LoadModel(string name)
    {
        var stream = await LoadStream(name);
        if (_objLoader == null)
        {
            var factory = new ObjLoaderFactory();
            _objLoader = factory.Create();
        }
        var result = _objLoader.Load(stream);

        var group = result.Groups.First();

        return new Model
        {
            Vertices = group.Faces.SelectMany(face =>
            {
                if (face.Count < 3) //is this even a face??
                    return [];

                var vertices = new Vertex[face.Count];

                for (var i = 0; i < face.Count; i++)
                {
                    var faceVertex = face[i];
                    Vector2 texCoord;

                    if (faceVertex.TextureIndex != 0)
                    {
                        var t = result.Textures[faceVertex.TextureIndex - 1];
                        texCoord = new Vector2(t.X, t.Y);
                    }
                    else
                    {
                        texCoord = new Vector2(0, 0);
                    }

                    var v = result.Vertices[faceVertex.VertexIndex - 1];
                    vertices[i] = new Vertex
                    {
                        Position = new Vector3(v.X, v.Y, v.Z),
                        Texcoord = texCoord
                    };
                }

                if (vertices.Length == 3)
                    return vertices; //fast path

                //https://www.youtube.com/watch?v=hTJFcHutls8

                //determine vector plane
                var v1 = vertices[0].Position - vertices[1].Position;
                var v2 = vertices[2].Position - vertices[1].Position;
                var faceNormal = Vector3.Cross(v1, v2);

                var projectedVertices = new Vector2[face.Count];

                //project vertices
                for (var i = 0; i < vertices.Length; i++)
                {
                    var v = vertices[i];

                    Vector3 projectedPoint = ProjectOntoPlane(v.Position, faceNormal);
                    Vector2 plane2DPoint = ConvertTo2D(projectedPoint, faceNormal);
                    projectedVertices[i] = plane2DPoint;
                }

                if (!Triangulate(projectedVertices, out int[] triangles, out string error))
                {
                    throw new Exception(error);
                }

                var resultV = new Vertex[triangles.Length];

                for (var i = 0; i < triangles.Length; i++)
                {
                    resultV[i] = vertices[triangles[i]];
                }

                return resultV;
            }).ToArray()
        };
    }

    private static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
    {
        triangles = [];
        errorMessage = "";

        if (vertices.Length < 3)
        {
            errorMessage = "At least 3 vertices per face requried";
            return false;
        }

        if (vertices.Length > 1024)
        {
            errorMessage = "Yooo, too many vertices in one face";
            return false;
        }

        // if (!IsSimplePolygon(vertices))
        // {
        //     errorMessage = "Not a simple polygon";
        //     return false;
        // }
        //
        // if (ContainsColinearEdges(vertices))
        // {
        //     errorMessage = "Contains colinear edges";
        //     return false;
        // }
        //
        // ComputePolygonArea(vertices, out float area, out WindingOrder windingOrder);
        //
        // if (windingOrder == WindingOrder.Invalid)
        // {
        //     return false;
        // }
        //
        // if (windingOrder == WindingOrder.CounterClockwise)
        // {
        //     Array.Reverse(vertices);
        // }

        var indexList = Enumerable.Range(0, vertices.Length).ToList();

        int totalTriangleCount = vertices.Length - 2;
        int totalTriangleIndexCount = totalTriangleCount * 3;

        triangles = new int[totalTriangleIndexCount];
        int triangleIndexCount = 0;

        while (indexList.Count > 3)
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                var a = indexList.GetAt(i);
                var b = indexList.GetAt(i - 1);
                var c = indexList.GetAt(i + 1);

                Vector2 av = vertices[a];
                Vector2 bv = vertices[b];
                Vector2 cv = vertices[c];

                var ab = bv - av;
                var ac = cv - av;

                //has to be convex
                if (Cross(ab, ac) < 0)
                    continue;

                bool isEar = true;

                //does ear contain any polygon vertex
                for (var j = 0; j < indexList.Count; j++)
                {
                    if(j == a || j == b || j == c)
                        continue;

                    Vector2 p = vertices[j];

                    if (IsPointInTriangle(bv, av, cv, p))
                    {
                        isEar = false;
                        break;
                    }
                }

                if (isEar)
                {
                    triangles[triangleIndexCount++] = b;
                    triangles[triangleIndexCount++] = a;
                    triangles[triangleIndexCount++] = c;
                    indexList.RemoveAt(i);
                    break;
                }
            }
        }

        triangles[triangleIndexCount++] = indexList[0];
        triangles[triangleIndexCount++] = indexList[1];
        triangles[triangleIndexCount] = indexList[2];

        return true;
    }

    private static bool IsPointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        var ab = b - a;
        var bc = c - b;
        var ca = a - c;

        var ap = p - a;
        var bp = p - b;
        var cp = p - c;

        if (Cross(ab, ap) > 0)
            return false;

        if (Cross(bc, bp) > 0)
            return false;

        if (Cross(ca, cp) > 0)
            return false;

        return true;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    private static bool ComputePolygonArea(Vector2[] vertices, out float area, out WindingOrder windingOrder)
    {
        area = 0;
        windingOrder = WindingOrder.Invalid;
        return false;
    }

    enum WindingOrder
    {
        Invalid,
        CounterClockwise
    }

    private static bool ContainsColinearEdges(Vector2[] vertices)
    {
        return false;
    }

    private static bool IsSimplePolygon(Vector2[] vertices)
    {
        return true;
    }

    private static Vector3 ProjectOntoPlane(Vector3 point, Vector3 normal)
    {
        // v_parallel = (v . n) * n
        Vector3 v_parallel = Vector3.Dot(point, normal) * normal;

        // v_perpendicular = v - v_parallel
        Vector3 v_perpendicular = point - v_parallel;

        return v_perpendicular;
    }

    private static Vector2 ConvertTo2D(Vector3 pointOnPlane, Vector3 planeNormal)
    {
        // Choose an arbitrary vector not aligned with the normal to create a basis vector
        Vector3 u1;
        if (planeNormal != Vector3.UnitX) u1 = Vector3.Cross(planeNormal, Vector3.UnitX);
        else u1 = Vector3.Cross(planeNormal, Vector3.UnitY);
        u1 = Vector3.Normalize(u1); // Normalize to get a unit vector

        // u2 is perpendicular to both planeNormal and u1
        Vector3 u2 = Vector3.Cross(planeNormal, u1);

        // Project the 3D point onto the 2D basis vectors u1 and u2
        float x = Vector3.Dot(pointOnPlane, u1);
        float y = Vector3.Dot(pointOnPlane, u2);

        return new Vector2(x, y);
    }
}