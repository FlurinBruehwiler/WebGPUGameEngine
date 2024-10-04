using System;
using System.Numerics;
using System.Threading.Tasks;
using WasmTestCSharp.WebGPU;
using Document = System.Reflection.Metadata.Document;

namespace WasmTestCSharp;

public static class Program
{
    public static GameInfo GameInfo = null!;

    public static async Task Main()
    {
        GameInfo = await InitializeGame();

        GameInfo.Camera = new Camera
        {
            Position = new Vector3(10, 0, 0),
            Rotation = new Vector3(0, MathF.PI / 2, 0)
        };

        JsWindow.RequestAnimationFrame(Frame);
    }

    private static void Frame()
    {
        const float velocity = 0.5f;

        if (GameInfo.Input.IsKeyDown("KeyW"))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Rotation.Y));
            GameInfo.Camera.Position += transformation.AsVector3() * -velocity;
        }
        if (GameInfo.Input.IsKeyDown("KeyS"))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Rotation.Y));
            GameInfo.Camera.Position += transformation.AsVector3() * velocity;
        }

        GameInfo.Camera.Rotation.Y += -GameInfo.Input.MouseChangeX * 0.001f;

        if (GameInfo.Input.IsKeyDown("KeyA"))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Rotation.Y));
            GameInfo.Camera.Position += transformation.AsVector3() * -velocity;
        }
        if (GameInfo.Input.IsKeyDown("KeyD"))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Rotation.Y));
            GameInfo.Camera.Position += transformation.AsVector3() * velocity;
        }

        if (GameInfo.Input.IsKeyDown("ShiftLeft"))
        {
            GameInfo.Camera.Position.Y -= velocity;
        }
        if (GameInfo.Input.IsKeyDown("Space"))
        {
            GameInfo.Camera.Position.Y += velocity;
        }

        Renderer.StartFrame();
            Renderer.DrawCube(new Vector3(10, 0, 0));
            Renderer.DrawCube(new Vector3(0, 0, 10));
            Renderer.DrawCube(new Vector3(-10, 0, 0));
            Renderer.DrawCube(new Vector3(0, 0, -10));
        Renderer.EndFrame(GameInfo.Camera);

        GameInfo.Input.NextFrame();

        //request next frame
        JsWindow.RequestAnimationFrame(Frame);
    }

    private static void HandleKeyEvent(string code, bool isDown)
    {
        GameInfo.Input.InformKeyChanged(code, isDown);
    }

    private static async Task<GameInfo> InitializeGame()
    {
        JsDocument.AddEventListener<KeyboardEvent>("keydown", e => HandleKeyEvent(e.Code, true));
        JsDocument.AddEventListener<KeyboardEvent>("keyup", e => HandleKeyEvent(e.Code, false));

        var canvas = JsDocument.QuerySelector<JsCanvas>("#gpuCanvas");

        canvas.AddEventListener<MouseEvent>("mousemove", e =>
        {
            GameInfo.Input.MouseChangeX += e.MovementX;
            GameInfo.Input.MouseChangeY += e.MovementY;
        });

        var adapter = await GPU.RequestAdapter();
        var device = await adapter.RequestDevice();

        var shaderModule = device.CreateShaderModule(new ShaderModuleDescriptor
        {
            Code = """
                   struct VertexOut {
                     @builtin(position) position : vec4f,
                     @location(0) color : vec4f
                   }

                   @binding(0)
                   @group(0)
                   var<uniform> viewMatrix : mat4x4<f32>;

                   @binding(1)
                   @group(0)
                   var<uniform> projectionMatrix : mat4x4<f32>;

                   @vertex
                   fn vertex_main(@location(0) position: vec4f, @location(1) color: vec4f) -> VertexOut
                   {
                     var output : VertexOut;
                   
                     //model view projection
                   
                     output.position = projectionMatrix * viewMatrix * position;
                     output.color = color;
                     return output;
                   }

                   @fragment
                   fn fragment_main(fragData: VertexOut) -> @location(0) vec4f
                   {
                     return fragData.color;
                   }
                   """
        });

        var presentationFormat = GPU.GetPreferredCanvasFormat();
        var context = canvas.GetContext();
        context.Configure(new ContextConfig
        {
            Device = device,
            Format = presentationFormat,
            AlphaMode = "premultiplied"
        });

        var vertexBuffer = new VertexBufferDescriptor
        {
            Attributes =
            [
                new VertexAttribute
                {
                    ShaderLocation = 0,
                    Offset = 0,
                    Format = "float32x4"
                },
                new VertexAttribute
                {
                    ShaderLocation = 1,
                    Offset = 16,
                    Format = "float32x4"
                }
            ],
            ArrayStride = 32,
            StepMode = "vertex"
        };

        var pipelineDescriptor = new RenderPipelineDescriptor
        {
            Vertex = new VertexDescriptor
            {
                Module = shaderModule,
                EntryPoint = "vertex_main",
                Buffers = [vertexBuffer]
            },
            Fragment = new FragmentDescriptor
            {
                Module = shaderModule,
                EntryPoint = "fragment_main",
                Targets = [new FragmentTarget
                {
                    Format = presentationFormat
                }]
            },
            Primitive = new PrimitiveDecsriptor
            {
                Topology = "triangle-list"
            },
            Layout = "auto"
        };

        var renderPipeline = device.CreateRenderPipeline(pipelineDescriptor);

        var gameInfo = new GameInfo
        {
            Device = device,
            JsCanvas = canvas,
            Context = context,
            RenderPipeline = renderPipeline,
        };

        gameInfo.UpdateScreenDimensions();

        return gameInfo;
    }
}
