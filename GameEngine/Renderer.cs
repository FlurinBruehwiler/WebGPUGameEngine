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

    public static GPUTexture CreateTextureFromBitmap(ImageBitmap imageBitmap)
    {
        var texture = Game.GameInfo.Device.CreateTexture(new TextureDescriptor
        {
            Format = "rgba8unorm",
            Usage = GPUTextureUsage.TEXTURE_BINDING |
                    GPUTextureUsage.COPY_DST |
                    GPUTextureUsage.RENDER_ATTACHMENT,
            Size = new SizeObject
            {
                Width = imageBitmap.Width,
                Height = imageBitmap.Height,
                DepthOrArrayLayers = 1
            }
        });

        Game.GameInfo.Device.Queue.CopyExternalImageToTexture(
            new ImageSource
            {
                Source = imageBitmap,
                FlipY = true
            },
            new TextureDestination
            {
                Texture = texture
            },
            new TextureSize
            {
                Width = imageBitmap.Width,
                Height = imageBitmap.Height
            });

        return texture;
    }

    public static GPUTexture CreateTexture(byte[] data, int width, int height)
    {
        var texture = Game.GameInfo.Device.CreateTexture(new TextureDescriptor
        {
            Format = "rgba8unorm",
            Usage = GPUTextureUsage.TEXTURE_BINDING | GPUTextureUsage.COPY_DST,
            Size = new SizeObject
            {
                Width = width,
                Height = height,
                DepthOrArrayLayers = 1
            }
        });

        Game.GameInfo.Device.Queue.WriteTexture(
            new TextureDestination
            {
                Texture = texture
            },
            data,
            new DataLayout
            {
                BytesPerRow = width * 4
            },
            new TextureSize
            {
                Width = width,
                Height = height
            });

        return texture;
    }

    public static void UploadModel(Model model)
    {
        var gameInfo = Game.GameInfo;

        var vertices = new double[model.Vertices.Length * 6];

        for (var i = 0; i < model.Vertices.Length; i++)
        {
            var vertex = model.Vertices[i];
            vertices[i * 6] = vertex.Position.X;
            vertices[i * 6 + 1] = vertex.Position.Y;
            vertices[i * 6 + 2] = vertex.Position.Z;
            vertices[i * 6 + 3] = 1;
            vertices[i * 6 + 4] = vertex.Texcoord.X;
            vertices[i * 6 + 5] = vertex.Texcoord.Y;
        }

        var vertexBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = vertices.Length * sizeof(float),
            Usage = GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
        });

        gameInfo.Device.Queue.WriteBuffer(vertexBuffer, 0, vertices, 0, vertices.Length);

        model.GpuBuffer = vertexBuffer;
    }

    // private static Model CreateImmediateModel()
    // {
    //     var immediateModel = new Model
    //     {
    //         Vertices = Game.GameInfo.ImmediateVertices.ToArray(),
    //         Texture = null
    //     };
    //
    //     UploadModel(immediateModel);
    //
    //     Game.GameInfo.ImmediateVertices = [];
    //     return immediateModel;
    // }

    public static void EndFrame(Camera camera)
    {
        var gameInfo = Game.GameInfo;

        gameInfo.UpdateScreenDimensions();

        // using var immediateModel = CreateImmediateModel();

        var commandEncoder = gameInfo.Device.CreateCommandEncoder();

        using var depthTexture = gameInfo.Device.CreateTexture(new TextureDescriptor
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

        const int uniformStride = 256;

        var uniformBufferSize =
            gameInfo.Entities.Count * uniformStride; //only works as long as each uniform is smaller than 256 bytes
        using var uniformBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = uniformBufferSize,
            Usage = GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

        for (var i = 0; i < gameInfo.Entities.Count; i++)
        {
            var entity = gameInfo.Entities[i];
            var modelMatrix = entity.Transform.ToMatrix();

            var uniforms = new Uniforms
            {
                ModelMatrix = modelMatrix,
                ProjectionMatrix = projectionMatrix,
                ViewMatrix = viewMatrix
            };

            var uniformData = uniforms.ToArray();
            gameInfo.Device.Queue.WriteBuffer(uniformBuffer, i * uniformStride, uniformData, 0, uniformData.Length);
        }

        var uniformBindGroup = gameInfo.Device.CreateBindGroup(new BindGroupDescriptor
        {
            // Layout = bindGroupLayout,
            Layout = gameInfo.RenderPipeline.GetBindGroupLayout(0),
            Entries =
            [
                new BindGroupEntry
                {
                    Binding = 0,
                    Resource = new BufferBinding
                    {
                        Buffer = uniformBuffer,
                        Offset = 0,
                        Size = uniformStride
                    }
                }
            ]
        });

        var passEncoder = commandEncoder.BeginRenderPass(renderPassDescriptor);

        passEncoder.SetPipeline(gameInfo.RenderPipeline);

        for (var i = 0; i < gameInfo.Entities.Count; i++)
        {
            var model = gameInfo.Entities[i].Model;

            model.TextureBindGroup ??= CreateTextureBindGroup(model);

            if (model.GpuBuffer != null)
            {
                passEncoder.SetBindGroup(0, uniformBindGroup, [i * uniformStride], 0, 1);
                passEncoder.SetBindGroup(1, model.TextureBindGroup);
                passEncoder.SetVertexBuffer(0, model.GpuBuffer);
                passEncoder.Draw(model.Vertices.Length);
            }
        }

        // if (immediateModel.GpuBuffer != null)
        // {
        //     passEncoder.SetVertexBuffer(0, immediateModel.GpuBuffer);
        //     passEncoder.Draw(immediateModel.Vertices.Length);
        // }

        passEncoder.End();

        var commandBuffer = commandEncoder.Finish();
        gameInfo.Device.Queue.Submit([commandBuffer]);
    }

    private static GPUBindGroup CreateTextureBindGroup(Model model)
    {
        var gameInfo = Game.GameInfo;

        var sampler = gameInfo.Device.CreateSampler();

        var info = new Info
        {
            Color = model.SolidColor,
            TextureType = model.Texture == null ? 0 : 1
        };

        var infoData = info.ToArray();

        //set color
        var infoBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = 256,
            Usage = GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });
        gameInfo.Device.Queue.WriteBuffer(infoBuffer, 0, infoData, 0, infoData.Length);

        return gameInfo.Device.CreateBindGroup(new BindGroupDescriptor
        {
            // Layout = bindGroupLayout,
            Layout = gameInfo.RenderPipeline.GetBindGroupLayout(1),
            Entries =
            [
                new BindGroupEntry
                {
                    Binding = 0,
                    Resource = sampler
                },
                new BindGroupEntry
                {
                    Binding = 1,
                    Resource = model.Texture?.GpuTexture.CreateView() ?? gameInfo.NullTexture.GpuTexture.CreateView()
                },
                new BindGroupEntry
                {
                    Binding = 2,
                    Resource = new BufferBinding
                    {
                        Buffer = infoBuffer
                    }
                }
            ]
        });
    }
}

public struct Info
{
    public required Color Color;
    public required int TextureType;

    public double[] ToArray()
    {
        var result = new double[8];
        result[0] = (double)Color.R / 255;
        result[1] = (double)Color.G / 255;
        result[2] = (double)Color.B / 255;
        result[3] = (double)Color.A / 255;
        result[4] = TextureType;
        return result;
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
    public static T GetAt<T>(this IList<T> list, int index)
    {
        if (index >= list.Count)
        {
            return list[index % list.Count];
        }

        if (index < 0)
        {
            return list[index % list.Count + list.Count];
        }

        return list[index];
    }

    public static double[] ToArray(this Color color)
    {
        return [(double)color.R / 255, (double)color.G / 255, (double)color.B / 255, (double)color.A / 255];
    }

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