using System;
using System.Numerics;
using System.Threading.Tasks;
using GameEngine;
using GameEngine.WebGPU;

namespace WasmTestCSharp;

public static class Program
{
    public static async Task Main()
    {
        Game.GameInfo = await InitializeGame();

        Game.GameInfo.Camera = new Camera
        {
            Transform = new Transform
            {
                Position = new Vector3(10, 0, 0),
                Scale = Vector3.One,
                Rotation = new Vector3(0, MathF.PI / 2, 0),
            }
        };

        var model = await ResourceManager.LoadModel("teapot.obj");
        Renderer.UploadModel(model);
        Game.GameInfo.Entities.Add(new Entity
        {
            Transform = Transform.Default(),
            Model = model,
        });

        JsWindow.RequestAnimationFrame(Frame);
    }

    private static void Frame()
    {
        const float velocity = 0.5f;

        if (Game.GameInfo.Input.IsKeyDown("KeyW"))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(Game.GameInfo.Camera.Transform.Rotation.Y));
            Game.GameInfo.Camera.Transform.Position += transformation.AsVector3() * -velocity;
        }
        if (Game.GameInfo.Input.IsKeyDown("KeyS"))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(Game.GameInfo.Camera.Transform.Rotation.Y));
            Game.GameInfo.Camera.Transform.Position += transformation.AsVector3() * velocity;
        }

        Game.GameInfo.Camera.Transform.Rotation.Y += -Game.GameInfo.Input.MouseChangeX * 0.001f;

        if (Game.GameInfo.Input.IsKeyDown("KeyA"))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(Game.GameInfo.Camera.Transform.Rotation.Y));
            Game.GameInfo.Camera.Transform.Position += transformation.AsVector3() * -velocity;
        }
        if (Game.GameInfo.Input.IsKeyDown("KeyD"))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(Game.GameInfo.Camera.Transform.Rotation.Y));
            Game.GameInfo.Camera.Transform.Position += transformation.AsVector3() * velocity;
        }

        if (Game.GameInfo.Input.IsKeyDown("ShiftLeft"))
        {
            Game.GameInfo.Camera.Transform.Position.Y -= velocity;
        }
        if (Game.GameInfo.Input.IsKeyDown("Space"))
        {
            Game.GameInfo.Camera.Transform.Position.Y += velocity;
        }

        Renderer.StartFrame();
            Renderer.DrawCube(new Vector3(10, 0, 0));
            // Renderer.DrawCube(new Vector3(0, 0, 10));
            // Renderer.DrawCube(new Vector3(-10, 0, 0));
            // Renderer.DrawCube(new Vector3(0, 0, -10));
        Renderer.EndFrame(Game.GameInfo.Camera);

        Game.GameInfo.Input.NextFrame();

        //request next frame
        JsWindow.RequestAnimationFrame(Frame);
    }

    private static void HandleKeyEvent(string code, bool isDown)
    {
        Game.GameInfo.Input.InformKeyChanged(code, isDown);
    }

    private static async Task<GameInfo> InitializeGame()
    {
        JsDocument.AddEventListener<KeyboardEvent>("keydown", e => HandleKeyEvent(e.Code, true));
        JsDocument.AddEventListener<KeyboardEvent>("keyup", e => HandleKeyEvent(e.Code, false));

        var canvas = JsDocument.QuerySelector<JsCanvas>("#gpuCanvas");

        canvas.AddEventListener<MouseEvent>("mousemove", e =>
        {
            Game.GameInfo.Input.MouseChangeX += e.MovementX;
            Game.GameInfo.Input.MouseChangeY += e.MovementY;
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
                   var<uniform> modelMatrix : mat4x4<f32>;
                   
                   @binding(1)
                   @group(0)
                   var<uniform> viewMatrix : mat4x4<f32>;

                   @binding(2)
                   @group(0)
                   var<uniform> projectionMatrix : mat4x4<f32>;

                   @vertex
                   fn vertex_main(@location(0) position: vec4f, @location(1) color: vec4f) -> VertexOut
                   {
                     var output : VertexOut;
                   
                     //model view projection
                   
                     output.position = projectionMatrix * viewMatrix * modelMatrix * position;
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
            Layout = "auto",
            DepthStencil = new DepthStencil
            {
                Format = "depth24plus",
                DepthCompare = "less",
                DepthWriteEnabled = true
            }
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
