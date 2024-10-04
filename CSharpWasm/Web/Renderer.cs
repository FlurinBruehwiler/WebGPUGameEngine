using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using WasmTestCSharp.WebGPU;

namespace WasmTestCSharp;

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
        Program.GameInfo.Vertices.Add(v1);
        Program.GameInfo.Vertices.Add(v2);
        Program.GameInfo.Vertices.Add(v3);
    }

    public static void StartFrame()
    {
        Program.GameInfo.Vertices = [];
    }

    public static void EndFrame(Camera camera)
    {
        var gameInfo = Program.GameInfo;

        gameInfo.UpdateScreenDimensions();

        var commandEncoder = gameInfo.Device.CreateCommandEncoder();

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
            ]
        };

        var vertices = gameInfo.Vertices.SelectMany(v => new List<double>
        {
            v.X, v.Y, v.Z, 1,
            1, 0.6f,1, 1
        }).ToArray();

        var vertexBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = vertices.Length * sizeof(float),
            Usage = GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
        });

        gameInfo.Device.Queue.WriteBuffer(vertexBuffer, 0, vertices, 0, vertices.Length);

        var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(60 * (MathF.PI / 180),
                                                                    (float)gameInfo.ScreenWidth / gameInfo.ScreenHeight,
                                                                    0.01f,
                                                                    10_000);

        var cameraModelMatrix = camera.GetMatrix();

        Matrix4x4.Invert(cameraModelMatrix, out var viewMatrix);

        var uniformBuffer = gameInfo.Device.CreateBuffer(new CreateBufferDescriptor
        {
            Size = 256 + 16 * sizeof(float),
            Usage = GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
        });

        gameInfo.Device.Queue.WriteBuffer(uniformBuffer, 0, viewMatrix.ToColumnMajorArray(), 0, 16);
        gameInfo.Device.Queue.WriteBuffer(uniformBuffer, 256, projectionMatrix.ToColumnMajorArray(), 0, 16);

        var bindGroup = gameInfo.Device.CreateBindGroup(new BindGroupDescriptor
        {
            Layout = gameInfo.RenderPipeline.GetBindGroupLayout(0),
            Entries =
            [
                new BindGroupEntry
                {
                    Binding = 0,
                    Resource = new EntryResource
                    {
                        Buffer = uniformBuffer,
                        Offset = 0,
                        Size = 16 * sizeof(float)
                    }
                },
                new BindGroupEntry
                {
                    Binding = 1,
                    Resource = new EntryResource
                    {
                        Buffer = uniformBuffer,
                        Offset = 256,
                        Size = 16 * sizeof(float)
                    }
                }
            ]
        });

        var passEncoder = commandEncoder.BeginRenderPass(renderPassDescriptor);

        passEncoder.SetPipeline(gameInfo.RenderPipeline);
        passEncoder.SetVertexBuffer(0, vertexBuffer);
        passEncoder.SetBindGroup(0, bindGroup);
        passEncoder.Draw(gameInfo.Vertices.Count);

        passEncoder.End();

        var commandBuffer = commandEncoder.Finish();
        gameInfo.Device.Queue.Submit([commandBuffer]);

        vertexBuffer.Destory();
    }
}

public class Camera
{
    public Vector3 Position;
    public Vector3 Rotation;

    public Matrix4x4 GetMatrix()
    {
        var cameraRotationMatrix =
            Matrix4x4.Multiply(
                Matrix4x4.CreateRotationX(Rotation.X),
                Matrix4x4.Multiply(Matrix4x4.CreateRotationY(Rotation.Y), Matrix4x4.CreateRotationZ(Rotation.Z)));

        return Matrix4x4.Multiply(cameraRotationMatrix, Matrix4x4.CreateTranslation(Position));
    }
}

public static class Extensions
{
    public static double[] ToColumnMajorArray(this Matrix4x4 matrix)
    {
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
