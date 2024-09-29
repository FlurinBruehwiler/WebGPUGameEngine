using System;
using System.Numerics;
using System.Threading.Tasks;
using WasmTestCSharp.WebGPU;

namespace WasmTestCSharp;

public static class Program
{
    public static GameInfo GameInfo = null!;

    public static async Task Main()
    {
        GameInfo = await InitializeGame();

        var camera = new Camera
        {
            Position = new Vector3(0, 0, 0),
            Rotation = new Vector3(0, 0, 0)
        };

        Renderer.StartFrame();
        Renderer.DrawCube(new Vector3(10, 0, 0));
        Renderer.DrawCube(new Vector3(0, 0, 10));
        Renderer.DrawCube(new Vector3(-10, 0, 0));
        Renderer.DrawCube(new Vector3(0, 0, -10));
        Renderer.EndFrame(camera);
    }

    private static async Task<GameInfo> InitializeGame()
    {
        var canvas = Canvas.Search("#gpuCanvas");
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
            Canvas = canvas,
            Context = context,
            RenderPipeline = renderPipeline,
        };

        gameInfo.UpdateScreenDimensions();

        return gameInfo;
    }
}
