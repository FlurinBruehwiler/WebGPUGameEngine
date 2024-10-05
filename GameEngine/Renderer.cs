using System.Drawing;
using System.Numerics;
using GameEngine.WebGPU;

namespace GameEngine;

public static class Renderer
{
    public static void DrawCube(Vector3 location)
    {
        var bottomBackLeft = new Vector3(0, 0, 0) + location;
        var bottomBackRight = new Vector3(1, 0, 0) + location;
        var bottomFrontLeft = new Vector3(0, 0, 1) + location;
        var bottomFrontRight = new Vector3(1, 0, 1) + location;

        var topBackLeft = new Vector3(0, 1, 0) + location;
        var topBackRight = new Vector3(1, 1, 0) + location;
        var topFrontLeft = new Vector3(0, 1, 1) + location;
        var topFrontRight = new Vector3(1, 1, 1) + location;

        DrawRectangle(
            bottomBackLeft,
            bottomBackRight,
            bottomFrontRight,
            bottomFrontLeft
        );
        DrawRectangle(
            bottomBackLeft,
            bottomBackRight,
            topBackRight,
            topBackLeft
        );
        DrawRectangle(
            topFrontLeft,
            topFrontRight,
            topBackRight,
            topBackLeft
        );
        DrawRectangle(
            topFrontLeft,
            topFrontRight,
            bottomFrontRight,
            bottomFrontLeft
        );
        DrawRectangle(
            topBackRight,
            topFrontRight,
            bottomFrontRight,
            bottomBackRight
        );
        DrawRectangle(
            topBackLeft,
            topFrontLeft,
            bottomFrontLeft,
            bottomBackLeft
        );
    }

    public static void DrawRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        DrawTriangle(v1, v2, v3);
        DrawTriangle(v1, v3, v4);
    }

    public static void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Game.GameInfo.ImmediateVertices.Add(v1);
        Game.GameInfo.ImmediateVertices.Add(v2);
        Game.GameInfo.ImmediateVertices.Add(v3);
    }

    public static void StartFrame()
    {
    }

    public static void UploadModel(Model model)
    {
        var gameInfo = Game.GameInfo;

        var vertices = new double[model.Vertices.Length * 8];

        for (var i = 0; i < model.Vertices.Length; i++)
        {
            var vertex = model.Vertices[i];
            vertices[i * 8] = vertex.X;
            vertices[i * 8 + 1] = vertex.Y;
            vertices[i * 8 + 2] = vertex.Z;
            vertices[i * 8 + 3] = 1;
            vertices[i * 8 + 4] = i % 3 == 0 ? 1 : 0;
            vertices[i * 8 + 5] = i % 3 == 1 ? 1 : 0;
            vertices[i * 8 + 6] = i % 3 == 2 ? 1 : 0;
            vertices[i * 8 + 7] = 1;
        }

        var vertexBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = vertices.Length * sizeof(float),
            Usage = GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
        });

        gameInfo.Device.Queue.WriteBuffer(vertexBuffer, 0, vertices, 0, vertices.Length);

        model.GpuBuffer = vertexBuffer;
    }

    private static Model CreateImmediateModel()
    {
        var immediateModel = new Model
        {
            Vertices = Game.GameInfo.ImmediateVertices.ToArray()
        };

        UploadModel(immediateModel);

        Game.GameInfo.ImmediateVertices = [];
        return immediateModel;
    }

    public static void EndFrame(Camera camera)
    {
        var gameInfo = Game.GameInfo;

        gameInfo.UpdateScreenDimensions();

        using var immediateModel = CreateImmediateModel();

        var commandEncoder = gameInfo.Device.CreateCommandEncoder();

        var depthTexture = gameInfo.Device.CreateTexture(new TextureDescriptor
        {
            Format = "depth24plus",
            Usage = GPUTextureUsage.RENDER_ATTACHMENT,
            Size = new SizeObject
            {
                Width = gameInfo.ScreenWidth,
                Height = gameInfo.ScreenHeight,
                DepthOrArrayLayers = 1
            }
        });

        var renderPassDescriptor = new RenderPassDescriptor
        {
            ColorAttachments =
            [
                new ColorAttachment
                {
                    ClearValue = Color.CornflowerBlue.ToColor(),
                    LoadOp = "clear",
                    StoreOp = "store",
                    View = gameInfo.Context.GetCurrentTexture().CreateView()
                }
            ],
            DepthStencilAttachment = new DepthStencilAttachment
            {
                View = depthTexture.CreateView(),
                DepthClearValue = 1.0f,
                DepthLoadOp = "clear",
                DepthStoreOp = "store"
            }
        };

        var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(60 * (MathF.PI / 180),
                                                                    (float)gameInfo.ScreenWidth / gameInfo.ScreenHeight,
                                                                    0.01f,
                                                                    10_000);

        var cameraModelMatrix = camera.Transform.ToMatrix();

        Matrix4x4.Invert(cameraModelMatrix, out var viewMatrix);

        var uniformBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = 256 + 256 + 16 * sizeof(float),
            Usage = GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

        var modelMatrix = Transform.Default(10).ToMatrix();

        var uniforms = new Uniforms
        {
            ModelMatrix = modelMatrix,
            ProjectionMatrix = projectionMatrix,
            ViewMatrix = viewMatrix
        };

        var uniformData = uniforms.ToArray();
        gameInfo.Device.Queue.WriteBuffer(uniformBuffer, 0, uniformData, 0, uniformData.Length);

        var bindGroupLayout = gameInfo.Device.CreateBindGroupLayout(new BindGroupLayoutDescriptor
        {
            Entries = [
                new LayoutEntry
                {
                    Binding = 0,
                    Visibility = GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT,
                    Buffer = new BufferBindingLayout
                    {
                        HasDynamicOffset = false
                    }
                }
            ]
        });

        var bindGroup = gameInfo.Device.CreateBindGroup(new BindGroupDescriptor
        {
            Layout = bindGroupLayout,
            // Layout = gameInfo.RenderPipeline.GetBindGroupLayout(0),
            Entries =
            [
                new BindGroupEntry
                {
                    Binding = 0,
                    Resource = new EntryResource
                    {
                        Buffer = uniformBuffer,
                        Offset = 0,
                        Size = uniformData.Length * sizeof(float)
                    }
                }
            ]
        });

        var passEncoder = commandEncoder.BeginRenderPass(renderPassDescriptor);

        passEncoder.SetPipeline(gameInfo.RenderPipeline);
        passEncoder.SetBindGroup(0, bindGroup);

        foreach (var entity in gameInfo.Entities)
        {
            var model = entity.Model;
            if (model.GpuBuffer != null)
            {
                passEncoder.SetVertexBuffer(0, model.GpuBuffer);
                passEncoder.Draw(model.Vertices.Length);
            }
        }

        if (immediateModel.GpuBuffer != null)
        {
            passEncoder.SetVertexBuffer(0, immediateModel.GpuBuffer);
            passEncoder.Draw(immediateModel.Vertices.Length);
        }

        passEncoder.End();

        var commandBuffer = commandEncoder.Finish();
        gameInfo.Device.Queue.Submit([commandBuffer]);
    }
}

public struct Uniforms
{
    public required Matrix4x4 ModelMatrix;
    public required Matrix4x4 ViewMatrix;
    public required Matrix4x4 ProjectionMatrix;

    public double[] ToArray()
    {
        var result = new double[16 * 3];
        ModelMatrix.ToColumnMajorArray().CopyTo(result.AsMemory(0, 16));
        ViewMatrix.ToColumnMajorArray().CopyTo(result.AsMemory(16, 16));
        ProjectionMatrix.ToColumnMajorArray().CopyTo(result.AsMemory(32, 16));
        return result;
    }
}

public class Camera
{
    public required Transform Transform;
}

public static class Extensions
{
    public static double[] ToColumnMajorArray(this Matrix4x4 matrix)
    {
        //id don't get it, this is row major, if I try column major, it doesn't work!!!!!!
        return
        [
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        ];
    }

    public static ClearColor ToColor(this Color color)
    {
        return new ClearColor
        {
            R = (float)color.R / 255,
            G = (float)color.G / 255,
            B = (float)color.B / 255,
            A = (float)color.A / 255
        };
    }
}
