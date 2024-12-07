using System.Text.Json.Serialization;

namespace Client.WebGPU;

public interface IGPUDevice
{
    IGPUQueue Queue { get; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createCommandEncoder
    /// </summary>
    IGPUCommandEncoder CreateCommandEncoder();

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBuffer
    /// </summary>
    IGPUBuffer CreateBuffer(CreateBufferDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createRenderPipeline
    /// </summary>
    IGPURenderPipeline CreateRenderPipeline(RenderPipelineDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBindGroup
    /// </summary>
    IGPUBindGroup CreateBindGroup(BindGroupDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBindGroupLayout
    /// </summary>
    IGPUBindGroupLayout CreateBindGroupLayout(BindGroupLayoutDescriptor descriptor);

    IGPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createTexture
    /// </summary>
    IGPUTexture CreateTexture(TextureDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createPipelineLayout
    /// </summary>
    IGPUPipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createSampler
    /// </summary>
    IGPUSampler CreateSampler(SamplerDescriptor descriptor);

    void PushErrorScope();
    void PopErrorScope();
}

public struct SamplerDescriptor
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AddressMode? AddressModeU { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AddressMode? AddressModeV { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AddressMode? AddressModeW { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FilterMode? MagFilter { get; set; }
}

public enum FilterMode
{
    Nearest = 0,
    Linear = 1,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public enum AddressMode
{
    Repeat = 0,
    MirrorRepeat = 1,
    ClampToEdge = 2,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public class PipelineLayoutDescriptor
{
    public required IGPUBindGroupLayout[] BindGroupLayouts { get; init; }
}

public class BindGroupLayoutDescriptor
{
    public required LayoutEntry[] Entries { get; init; }
    public string? Label;
}

public class LayoutEntry
{
    public required int Binding { get; init; }
    public required GPUShaderStage Visibility { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BufferBindingLayout? Buffer { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SamplerBindingLayout? Sampler { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TextureBindingLayout? Texture { get; set; }
}

public struct SamplerBindingLayout;

public struct TextureBindingLayout;


public class BufferBindingLayout
{
    public bool HasDynamicOffset { get; set; }
}

[Flags]
public enum GPUShaderStage
{
    FRAGMENT = 2,
    VERTEX = 1,
    COMPUTE = 4
}

public class TextureDescriptor
{
    public required GPUTextureFormat Format { get; set; }
    public required SizeObject Size { get; set; }
    public required GPUTextureUsage Usage { get; set; }
}

public class SizeObject
{
    public required int Width { get; set; }
    public required int Height { get; set; }
    public required int DepthOrArrayLayers { get; set; }
}

public class BindGroupDescriptor
{
    public required IGPUBindGroupLayout Layout { get; set; }
    public required BindGroupEntry[] Entries { get; set; }
}

public class BindGroupEntry
{
    public required int Binding { get; init; }
    public required IBindGroupResource Resource { get; init; }
}

[JsonDerivedType(typeof(BufferBinding))]
[JsonDerivedType(typeof(IGPUSampler))]
[JsonDerivedType(typeof(IGPUTextureView))]
public interface IBindGroupResource;

public class BufferBinding : IBindGroupResource
{
    public required IGPUBuffer Buffer { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Offset { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Size { get; set; }
}

public struct ShaderModuleDescriptor
{
    public required string Code { get; init; }
}

public struct RenderPipelineDescriptor
{
    public required VertexDescriptor Vertex { get; set; }
    public required FragmentDescriptor Fragment { get; set; }
    public required PrimitiveDescriptor Primitive { get; set; }
    public required IGPUPipelineLayout Layout { get; set; }
    public DepthStencil? DepthStencil { get; set; }
}

public struct DepthStencil
{
    public required CompareFunction DepthCompare { get; set; }
    public required bool DepthWriteEnabled { get; set; }
    public required GPUTextureFormat Format { get; set; }
}

public enum CompareFunction
{
    Undefined = 0,
    Never = 1,
    Less = 2,
    LessEqual = 3,
    Greater = 4,
    GreaterEqual = 5,
    Equal = 6,
    NotEqual = 7,
    Always = 8,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public struct PrimitiveDescriptor
{
    public PrimitiveTopology Topology { get; set; }
}

public enum PrimitiveTopology
{
    PointList = 0,
    LineList = 1,
    LineStrip = 2,
    TriangleList = 3,
    TriangleStrip = 4,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public struct FragmentDescriptor
{
    public IGPUShaderModule Module { get; set; }
    public string EntryPoint { get; set; }
    public FragmentTarget[] Targets { get; set; }
}

public struct FragmentTarget
{
    public GPUTextureFormat Format { get; set; }
}

public struct VertexDescriptor
{
    public IGPUShaderModule Module { get; set; }
    public string EntryPoint { get; set; }
    public VertexBufferDescriptor[] Buffers { get; set; }
}

public struct VertexBufferDescriptor
{
    public int ArrayStride { get; set; }
    public VertexStepMode StepMode { get; set; }
    public VertexAttribute[] Attributes { get; set; }
}

public enum VertexStepMode
{
    Vertex = 0,
    Instance = 1,
    VertexBufferNotUsed = 2,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public struct VertexAttribute
{
    public int ShaderLocation { get; set; }
    public int Offset { get; set; }
    public VertexFormat Format { get; set; }
}

public enum GPUTextureFormat
{
    Undefined = 0,
    R8Unorm = 1,
    R8Snorm = 2,
    R8Uint = 3,
    R8Sint = 4,
    R16Uint = 5,
    R16Sint = 6,
    R16float = 7,
    RG8Unorm = 8,
    RG8Snorm = 9,
    RG8Uint = 10, // 0x0000000A
    RG8Sint = 11, // 0x0000000B
    R32float = 12, // 0x0000000C
    R32Uint = 13, // 0x0000000D
    R32Sint = 14, // 0x0000000E
    RG16Uint = 15, // 0x0000000F
    RG16Sint = 16, // 0x00000010
    RG16float = 17, // 0x00000011
    Rgba8Unorm = 18, // 0x00000012
    Rgba8UnormSrgb = 19, // 0x00000013
    Rgba8Snorm = 20, // 0x00000014
    Rgba8Uint = 21, // 0x00000015
    Rgba8Sint = 22, // 0x00000016
    Bgra8Unorm = 23, // 0x00000017
    Bgra8UnormSrgb = 24, // 0x00000018
    Rgb10A2Uint = 25, // 0x00000019
    Rgb10A2Unorm = 26, // 0x0000001A
    RG11B10Ufloat = 27, // 0x0000001B
    Rgb9E5Ufloat = 28, // 0x0000001C
    RG32float = 29, // 0x0000001D
    RG32Uint = 30, // 0x0000001E
    RG32Sint = 31, // 0x0000001F
    Rgba16Uint = 32, // 0x00000020
    Rgba16Sint = 33, // 0x00000021
    Rgba16float = 34, // 0x00000022
    Rgba32float = 35, // 0x00000023
    Rgba32Uint = 36, // 0x00000024
    Rgba32Sint = 37, // 0x00000025
    Stencil8 = 38, // 0x00000026
    Depth16Unorm = 39, // 0x00000027
    Depth24Plus = 40, // 0x00000028
    Depth24PlusStencil8 = 41, // 0x00000029
    Depth32float = 42, // 0x0000002A
    Depth32floatStencil8 = 43, // 0x0000002B
    BC1RgbaUnorm = 44, // 0x0000002C
    BC1RgbaUnormSrgb = 45, // 0x0000002D
    BC2RgbaUnorm = 46, // 0x0000002E
    BC2RgbaUnormSrgb = 47, // 0x0000002F
    BC3RgbaUnorm = 48, // 0x00000030
    BC3RgbaUnormSrgb = 49, // 0x00000031
    BC4RUnorm = 50, // 0x00000032
    BC4RSnorm = 51, // 0x00000033
    BC5RGUnorm = 52, // 0x00000034
    BC5RGSnorm = 53, // 0x00000035
    BC6HrgbUfloat = 54, // 0x00000036
    BC6HrgbFloat = 55, // 0x00000037
    BC7RgbaUnorm = 56, // 0x00000038
    BC7RgbaUnormSrgb = 57, // 0x00000039
    Etc2Rgb8Unorm = 58, // 0x0000003A
    Etc2Rgb8UnormSrgb = 59, // 0x0000003B
    Etc2Rgb8A1Unorm = 60, // 0x0000003C
    Etc2Rgb8A1UnormSrgb = 61, // 0x0000003D
    Etc2Rgba8Unorm = 62, // 0x0000003E
    Etc2Rgba8UnormSrgb = 63, // 0x0000003F
    Eacr11Unorm = 64, // 0x00000040
    Eacr11Snorm = 65, // 0x00000041
    Eacrg11Unorm = 66, // 0x00000042
    Eacrg11Snorm = 67, // 0x00000043
    Astc4x4Unorm = 68, // 0x00000044
    Astc4x4UnormSrgb = 69, // 0x00000045
    Astc5x4Unorm = 70, // 0x00000046
    Astc5x4UnormSrgb = 71, // 0x00000047
    Astc5x5Unorm = 72, // 0x00000048
    Astc5x5UnormSrgb = 73, // 0x00000049
    Astc6x5Unorm = 74, // 0x0000004A
    Astc6x5UnormSrgb = 75, // 0x0000004B
    Astc6x6Unorm = 76, // 0x0000004C
    Astc6x6UnormSrgb = 77, // 0x0000004D
    Astc8x5Unorm = 78, // 0x0000004E
    Astc8x5UnormSrgb = 79, // 0x0000004F
    Astc8x6Unorm = 80, // 0x00000050
    Astc8x6UnormSrgb = 81, // 0x00000051
    Astc8x8Unorm = 82, // 0x00000052
    Astc8x8UnormSrgb = 83, // 0x00000053
    Astc10x5Unorm = 84, // 0x00000054
    Astc10x5UnormSrgb = 85, // 0x00000055
    Astc10x6Unorm = 86, // 0x00000056
    Astc10x6UnormSrgb = 87, // 0x00000057
    Astc10x8Unorm = 88, // 0x00000058
    Astc10x8UnormSrgb = 89, // 0x00000059
    Astc10x10Unorm = 90, // 0x0000005A
    Astc10x10UnormSrgb = 91, // 0x0000005B
    Astc12x10Unorm = 92, // 0x0000005C
    Astc12x10UnormSrgb = 93, // 0x0000005D
    Astc12x12Unorm = 94, // 0x0000005E
    Astc12x12UnormSrgb = 95, // 0x0000005F
    Force32 = 2147483647, // 0x7FFFFFFF
}

public enum VertexFormat
{
    Undefined = 0,
    Uint8x2 = 1,
    Uint8x4 = 2,
    Sint8x2 = 3,
    Sint8x4 = 4,
    Unorm8x2 = 5,
    Unorm8x4 = 6,
    Snorm8x2 = 7,
    Snorm8x4 = 8,
    Uint16x2 = 9,
    Uint16x4 = 10, // 0x0000000A
    Sint16x2 = 11, // 0x0000000B
    Sint16x4 = 12, // 0x0000000C
    Unorm16x2 = 13, // 0x0000000D
    Unorm16x4 = 14, // 0x0000000E
    Snorm16x2 = 15, // 0x0000000F
    Snorm16x4 = 16, // 0x00000010
    Float16x2 = 17, // 0x00000011
    Float16x4 = 18, // 0x00000012
    Float32 = 19, // 0x00000013
    Float32x2 = 20, // 0x00000014
    Float32x3 = 21, // 0x00000015
    Float32x4 = 22, // 0x00000016
    Uint32 = 23, // 0x00000017
    Uint32x2 = 24, // 0x00000018
    Uint32x3 = 25, // 0x00000019
    Uint32x4 = 26, // 0x0000001A
    Sint32 = 27, // 0x0000001B
    Sint32x2 = 28, // 0x0000001C
    Sint32x3 = 29, // 0x0000001D
    Sint32x4 = 30, // 0x0000001E
    Force32 = 2147483647, // 0x7FFFFFFF
}

public struct CreateBufferDescriptor
{
    public required int Size { get; init; }
    public required GPUBufferUsage Usage { get; init; }
}

[Flags]
public enum GPUBufferUsage
{
    COPY_SRC = 4,
    COPY_DST = 8,
    INDEX = 16,
    INDIRECT = 256,
    MAP_READ = 1,
    MAP_WRITE = 2,
    QUERY_RESOLVE = 512,
    STORAGE = 128,
    UNIFORM = 64,
    VERTEX = 32,
}

[Flags]
public enum GPUTextureUsage
{
    COPY_SRC = 1,
    COPY_DST = 2,
    RENDER_ATTACHMENT = 16,
    STORE_BINDING = 8,
    TEXTURE_BINDING = 4
}
