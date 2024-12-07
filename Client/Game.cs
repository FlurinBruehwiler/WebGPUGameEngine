using System.Numerics;
using Client.WebGPU;

namespace Client;

public class Game
{
    public static GameInfo GameInfo = null!;
    public static bool StartedUp() => GameInfo != null!;

    public static async Task<GameInfo> InitializeGame(IPlatformImplementation platformImplementation, IGPUDevice device, GPUTextureFormat textureFormat)
    {
        var resourceManager = new ResourceManager(platformImplementation);

        var shaderModule = device.CreateShaderModule(new ShaderModuleDescriptor
        {
            Code = await resourceManager.LoadString("shaders.wgsl")
        });

        var vertexBuffer = new VertexBufferDescriptor
        {
            Attributes =
            [
                new VertexAttribute //position
                {
                    ShaderLocation = 0,
                    Offset = 0,
                    Format = VertexFormat.Float32x4
                },
                new VertexAttribute //texcoord
                {
                    ShaderLocation = 1,
                    Offset = 16,
                    Format = VertexFormat.Float32x2
                }
            ],
            ArrayStride = 24,
            StepMode = VertexStepMode.Vertex
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
                    Format = textureFormat
                }]
            },
            Primitive = new PrimitiveDescriptor
            {
                Topology = PrimitiveTopology.TriangleList
            },
            Layout = device.CreatePipelineLayout(new PipelineLayoutDescriptor
            {
                BindGroupLayouts = [uniformBindGroupLayout, textureBindGroupLayout]
            }),
            // DepthStencil = new DepthStencil
            // {
            //     Format = GPUTextureFormat.Depth24Plus,
            //     DepthCompare = CompareFunction.Less,
            //     DepthWriteEnabled = true
            // }
        };

        var renderPipeline = device.CreateRenderPipeline(pipelineDescriptor);
        
        var gameInfo = new GameInfo
        {
            Device = device,
            // Screen = new Screen
            // {
            //     Canvas = canvas
            // },
            PlatformImplementation = platformImplementation,
            RenderPipeline = renderPipeline,
            NullTexture = null!,
            ResourceManager = resourceManager
        };


        var server = new Server();
        if (await server.TryEstablishConnection())
        {
            Console.WriteLine("running in online mode");
            gameInfo.Server = server;
        }
        else
        {
            Console.WriteLine("running in offline mode");
        }

        gameInfo.UpdateScreenDimensions();

        return gameInfo;
    }

    public static void Frame()
    {
        GameInfo.Server?.ProcessServerMessages();

        UpdateCamera();

        GameInfo.Server?.SendUpdatesToServer();

        _ = GameInfo.Server?.SendPendingMessages(); //async

        Renderer.RenderFrame(GameInfo.Camera);

        GameInfo.Input.NextFrame();
    }

    public static Guid PlayerGuid = Guid.NewGuid();

    public static Vector3 RandomVector(float from, float to)
    {
        var x = from + to * Random.Shared.NextSingle();
        var y = from + to * Random.Shared.NextSingle();
        var z = from + to * Random.Shared.NextSingle();
        return new Vector3(x, y, z);
    }

    private static void UpdateCamera()
    {
        const float velocity = 0.5f;

        if (GameInfo.Input.IsKeyDown(KeyboardKeys.W))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Transform.Rotation.Y));
            GameInfo.Camera.Transform.Position += transformation.AsVector3() * -velocity;
        }
        if (GameInfo.Input.IsKeyDown(KeyboardKeys.S))
        {
            var transformation = Vector4.Transform(new Vector4(0, 0, 1, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Transform.Rotation.Y));
            GameInfo.Camera.Transform.Position += transformation.AsVector3() * velocity;
        }

        GameInfo.Camera.Transform.Rotation.Y += -GameInfo.Input.MouseChangeX * 0.001f;

        if (GameInfo.Input.IsKeyDown(KeyboardKeys.A))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Transform.Rotation.Y));
            GameInfo.Camera.Transform.Position += transformation.AsVector3() * -velocity;
        }
        if (GameInfo.Input.IsKeyDown(KeyboardKeys.D))
        {
            var transformation = Vector4.Transform(new Vector4(1, 0, 0, 0), Matrix4x4.CreateRotationY(GameInfo.Camera.Transform.Rotation.Y));
            GameInfo.Camera.Transform.Position += transformation.AsVector3() * velocity;
        }

        if (GameInfo.Input.IsKeyDown(KeyboardKeys.ShiftLeft))
        {
            GameInfo.Camera.Transform.Position.Y -= velocity;
        }
        if (GameInfo.Input.IsKeyDown(KeyboardKeys.Space))
        {
            GameInfo.Camera.Transform.Position.Y += velocity;
        }
    }
}