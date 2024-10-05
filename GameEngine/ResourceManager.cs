using System.Globalization;
using System.Numerics;

namespace GameEngine;

public class ResourceManager
{
    public static HttpClient HttpClient = new();

    public static async Task<Model> LoadModel(string name)
    {
        string baseUrl;
#if DEBUG
        baseUrl = "https://localhost:7030/";
#else
        baseUrl = "https://flurinbruehwiler.github.io/WebGPUGameEngine/";
#endif

        var obj = await HttpClient.GetStringAsync(baseUrl + name);
        // var obj = File.ReadAllText(@"C:\Programming\Github\webgputest\GameEngine\Resources\teapot.obj");

        return Parse(obj);
    }

    private static Model Parse(string objFile)
    {
        List<Vector3> vertices = [];
        List<Face> faces = [];

        var span = objFile.AsSpan();

        while (true)
        {
            var lineEnd = span.IndexOfAny('\n', '\r');

            if (lineEnd == 0)
            {
                EatNewLine(ref span);
                continue;
            }

            ReadOnlySpan<char> line = span;

            if (lineEnd != -1)
            {
                line = span.Slice(0, lineEnd);
                span = span.Slice(lineEnd);
                EatNewLine(ref span);

                ProcessLine(line, vertices, faces);
            }
            else
            {
                ProcessLine(line, vertices, faces);
                var finalVertices = new Vector3[faces.Count * 3];
                for (var i = 0; i < faces.Count; i++)
                {
                    var face = faces[i];
                    finalVertices[i * 3] = vertices[face.V1 - 1];
                    finalVertices[i * 3 + 1] = vertices[face.V2 - 1];
                    finalVertices[i * 3 + 2] = vertices[face.V3 - 1];
                }
                return new Model
                {
                    Vertices = finalVertices
                };
            }
        }
    }

    private static void EatNewLine(ref ReadOnlySpan<char> span)
    {
        while (span.Length > 0 && ( span[0] == '\r' || span[0] != '\n' || span[0] != '\n'))
        {
            span = span.Slice(1);
        }
    }

    private static void EatWhitespace(ref ReadOnlySpan<char> span)
    {
        while (span.Length > 0 && span[0] == ' ')
        {
            span = span.Slice(1);
        }
    }

    private static void ProcessLine(ReadOnlySpan<char> line, List<Vector3> vertices, List<Face> faces)
    {
        if (line[0] == 'v')
        {
            line = line.Slice(1);
            EatWhitespace(ref line);
            vertices.Add(ParseVector3(line));
        }
        else if (line[0] == 'f')
        {
            line = line.Slice(1);
            EatWhitespace(ref line);
            faces.Add(ParseFace(line));
        }
    }

    private static Vector3 ParseVector3(ReadOnlySpan<char> span)
    {
        var v1 = ParseFloat(ref span);
        var v2 = ParseFloat(ref span);
        var v3 = ParseFloat(ref span);

        return new Vector3(v1, v2, v3);
    }

    private static float ParseFloat(ref ReadOnlySpan<char> span)
    {
        var idx = span.IndexOf(' ');
        if (idx != -1)
        {
            var f = float.Parse(span.Slice(0, idx));
            span = span.Slice(idx + 1);
            return f;
        }

        return float.Parse(span);
    }

    private static int ParseInt(ref ReadOnlySpan<char> span)
    {
        var idx = span.IndexOf(' ');
        if (idx != -1)
        {
            var i = int.Parse(span.Slice(0, idx));
            span = span.Slice(idx + 1);
            return i;
        }

        return int.Parse(span);
    }

    private static Face ParseFace(ReadOnlySpan<char> span)
    {
        var v1 = ParseInt(ref span);
        var v2 = ParseInt(ref span);
        var v3 = ParseInt(ref span);

        return new Face
        {
            V1 = v1,
            V2 = v2,
            V3 = v3
        };
    }
}

public struct Face
{
    public int V1;
    public int V2;
    public int V3;
}