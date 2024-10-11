using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GameEngine;
using GameEngine.WebGPU;
using Shared;

namespace WasmTestCSharp;

public static class Program
{
    public static async Task Main()
    {
        try
        {
            Game.GameInfo = await InitializeGame();

            Game.GameInfo.NullTexture = await Game.GameInfo.ResourceManager.LoadTexture("NullTexture.png");

            Game.GameInfo.Camera = new Camera
            {
                Transform = new Transform
                {
                    Position = new Vector3(10, 0, 0),
                    Scale = Vector3.One,
                    Rotation = new Vector3(0, MathF.PI / 2, 0),
                }
            };

            var cubeModel = await Game.GameInfo.ResourceManager.LoadModel("crate.obj");
            Renderer.UploadModel(cubeModel);
            cubeModel.SolidColor = Color.Beige;

            // cubeModel.Texture = await ResourceManager.LoadTexture("crate-texture.jpg");

            for (int i = 0; i < 10; i++)
            {
                Game.GameInfo.Entities.Add(new Entity
                {
                    Transform = new Transform
                    {
                        Scale = Vector3.One,
                        Position = RandomVector(-25, 25) with{ Y = 1}
                    },
                    Model = cubeModel,
                });
            }

            var planeModel = await Game.GameInfo.ResourceManager.LoadModel("plane.obj");
            Renderer.UploadModel(planeModel);
            planeModel.Texture = await Game.GameInfo.ResourceManager.LoadTexture("grass.png");
            planeModel.SolidColor = Color.Green;

            Game.GameInfo.Entities.Add(new Entity
            {
                Transform = Transform.Default(30) with
                {
                    Position = new Vector3(-10, 0, -10)
                },
                Model = planeModel
            });

            Game.GameInfo.Server?.ListenForMessages();

            JsWindow.RequestAnimationFrame(FrameCatch);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Vector3 RandomVector(float from, float to)
    {
        var x = from + to * Random.Shared.NextSingle();
        var y = from + to * Random.Shared.NextSingle();
        var z = from + to * Random.Shared.NextSingle();
        return new Vector3(x, y, z);
    }

    private static void FrameCatch()
    {
        try
        {
            Frame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Guid PlayerGuid = Guid.NewGuid();

    private static void SendUpdatesToServer()
    {
        if (Game.GameInfo.Server == null)
            return;

        var server = Game.GameInfo.Server;

        server.PendingSendingMessages.Enqueue(new UpdateMessage
        {
            EntityId = PlayerGuid,
            Transform = Game.GameInfo.Camera.Transform.ToNetwork()
        });
    }

    private static void ProcessServerMessages()
    {
        if (Game.GameInfo.Server == null)
            return;

        var server = Game.GameInfo.Server;

        while (server.PendingReceivingMessages.TryDequeue(out var message))
        {
            if (message is UpdateMessage updateMessage)
            {
                var entity = Game.GameInfo.Entities.First(x => x.Id == updateMessage.EntityId);
                entity.Transform = Transform.FromNetwork(updateMessage.Transform);
            }
            else if (message is CreateMessage createMessage)
            {
                Game.GameInfo.Entities.Add(new Entity
                {
                    Id = createMessage.EntityId,
                    Transform = Transform.FromNetwork(createMessage.Transform),
                    Model = Game.GameInfo.ResourceManager.GetModelFromId(createMessage.ModelId)
                });
            }
        }
    }

    private static void Frame()
    {
        ProcessServerMessages();

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

        SendUpdatesToServer();

        Renderer.StartFrame();
            Renderer.DrawCube(new Vector3(10, 0, 0));
            // Renderer.DrawCube(new Vector3(0, 0, 10));
            // Renderer.DrawCube(new Vector3(-10, 0, 0));
            // Renderer.DrawCube(new Vector3(0, 0, -10));
        Renderer.EndFrame(Game.GameInfo.Camera);

        Game.GameInfo.Input.NextFrame();

        //request next frame
        JsWindow.RequestAnimationFrame(FrameCatch);
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
            if (!Game.StartedUp())
                return;

            Game.GameInfo.Input.MouseChangeX += e.MovementX;
            Game.GameInfo.Input.MouseChangeY += e.MovementY;
        });

        var adapter = await GPU.RequestAdapter();
        var device = await adapter.RequestDevice();

        var resourceManager = new ResourceManager();

        var shaderModule = device.CreateShaderModule(new ShaderModuleDescriptor
        {
            Code = await resourceManager.LoadString("shaders.wgsl")
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
                new VertexAttribute //position
                {
                    ShaderLocation = 0,
                    Offset = 0,
                    Format = "float32x4"
                },
                new VertexAttribute //texcoord
                {
                    ShaderLocation = 1,
                    Offset = 16,
                    Format = "float32x2"
                }
            ],
            ArrayStride = 24,
            StepMode = "vertex"
        };

        var uniformBindGroupLayout = device.CreateBindGroupLayout(new BindGroupLayoutDescriptor
        {
            Entries = [
                new LayoutEntry
                {
                    Binding = 0,
                    Visibility = GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT,
                    Buffer = new BufferBindingLayout
                    {
                        HasDynamicOffset = true
                    }
                }
            ]
        });

        var textureBindGroupLayout = device.CreateBindGroupLayout(new BindGroupLayoutDescriptor
        {
            Entries = [
                new LayoutEntry
                {
                    Binding = 0,
                    Visibility = GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT,
                    Sampler = new SamplerBindingLayout()
                },
                new LayoutEntry
                {
                    Binding = 1,
                    Visibility = GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT,
                    Texture = new TextureBindingLayout()
                },
                new LayoutEntry
                {
                    Binding = 2,
                    Visibility = GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT,
                    Buffer = new BufferBindingLayout()
                }
            ]
        });

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
            Layout = device.CreatePipelineLayout(new PipelineLayoutDescriptor
            {
                BindGroupLayouts = [uniformBindGroupLayout, textureBindGroupLayout]
            }),
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
            NullTexture = null!,
            ResourceManager = resourceManager
        };

        gameInfo.UpdateScreenDimensions();

        return gameInfo;
    }
}
