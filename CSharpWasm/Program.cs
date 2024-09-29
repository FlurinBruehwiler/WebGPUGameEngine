using WasmTestCSharp.WebGPU;

var canvas = Canvas.Search("#gpuCanvas");
var adapter = await GPU.RequestAdapter();
var device = await adapter.RequestDevice();

var presentationFormat = GPU.GetPreferredCanvasFormat();
var context = canvas.GetContext();
context.Configure(new ContextConfig
{
    Device = device,
    Format = presentationFormat,
    AlphaMode = AlphaMode.Premultiplied
});

var shaderModule = device.CreateShaderModule(new ShaderModuleDescriptor
{
    Code = ""
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
    StepMode = StepMode.Vertex
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
        Topology = Topology.TriangleList
    },
    Layout = "auto"
};

var renderPipeline = device.CreateRenderPipeline(pipelineDescriptor);

