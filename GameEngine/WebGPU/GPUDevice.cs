using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice
/// </summary>
public class GPUDevice : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public GPUQueue Queue
    {
        get
        {
            var queueJsObject = JsObject.GetPropertyAsJSObject("queue");

            if (queueJsObject == null)
                throw new Exception("Error");

            if (queueJsObject != cachedGPUQueue?.JsObject)
            {
                return cachedGPUQueue = new GPUQueue
                {
                    JsObject = queueJsObject
                };
            }

            return cachedGPUQueue;
        }
    }
    private GPUQueue? cachedGPUQueue;

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createCommandEncoder
    /// </summary>
    public GPUCommandEncoder CreateCommandEncoder()
    {
        return new GPUCommandEncoder
        {
            JsObject = Interop.GPUDevice_CreateCommandEncoder(JsObject)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBuffer
    /// </summary>
    public GPUBuffer CreateBuffer(CreateBufferDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUBuffer
        {
            JsObject = Interop.GPUDevice_CreateBuffer(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createRenderPipeline
    /// </summary>
    public GPURenderPipeline CreateRenderPipeline(RenderPipelineDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPURenderPipeline
        {
            JsObject = Interop.GPUDevice_CreateRenderPipeline(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBindGroup
    /// </summary>
    public GPUBindGroup CreateBindGroup(BindGroupDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUBindGroup
        {
            JsObject = Interop.GPUDevice_CreateBindGroup(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBindGroupLayout
    /// </summary>
    public GPUBindGroupLayout CreateBindGroupLayout(BindGroupLayoutDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUBindGroupLayout
        {
            JsObject = Interop.GPUDevice_CreateBindGroupLayout(JsObject, json, references)
        };
    }

    public GPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUShaderModule
        {
            JsObject = Interop.GPUDevice_CreateShaderModule(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createTexture
    /// </summary>
    public GPUTexture CreateTexture(TextureDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUTexture
        {
            JsObject = Interop.GPUDevice_CreateTexture(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createPipelineLayout
    /// </summary>
    public GPUPipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUPipelineLayout
        {
            JsObject = Interop.GPUDevice_CreatePipelineLayout(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createSampler
    /// </summary>
    public GPUSampler CreateSampler() //can also have a descriptor
    {
        return new GPUSampler
        {
            JsObject = Interop.GPUDevice_CreateSampler(JsObject)
        };
    }
}

public class PipelineLayoutDescriptor
{
    public required GPUBindGroupLayout[] BindGroupLayouts { get; init; }
}

public class BindGroupLayoutDescriptor
{
    public required LayoutEntry[] Entries { get; init; }
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
    public required string Format { get; set; }
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
    public required GPUBindGroupLayout Layout { get; set; }
    public required BindGroupEntry[] Entries { get; set; }
}

public class BindGroupEntry
{
    public required int Binding { get; init; }
    public required IBindGroupResource Resource { get; init; }
}

[JsonDerivedType(typeof(BufferBinding))]
[JsonDerivedType(typeof(GPUSampler))]
[JsonDerivedType(typeof(GPUTextureView))]
public interface IBindGroupResource;

public class BufferBinding : IBindGroupResource
{
    public required GPUBuffer Buffer { get; init; }
    public int Offset { get; set; }
    public int Size { get; set; }
}

public class ShaderModuleDescriptor
{
    public required string Code { get; init; }
}

public class RenderPipelineDescriptor
{
    public required VertexDescriptor Vertex { get; set; }
    public required FragmentDescriptor Fragment { get; set; }
    public required PrimitiveDecsriptor Primitive { get; set; }
    public required GPUPipelineLayout Layout { get; set; }
    public required DepthStencil DepthStencil { get; set; }
}

public class DepthStencil
{
    public required string DepthCompare { get; set; }
    public required bool DepthWriteEnabled { get; set; }
    public required string Format { get; set; }
}

public class PrimitiveDecsriptor
{
    public string Topology { get; set; }
}

public class FragmentDescriptor
{
    public GPUShaderModule Module { get; set; }
    public string EntryPoint { get; set; }
    public FragmentTarget[] Targets { get; set; }
}

public class FragmentTarget
{
    public string Format { get; set; }
}

public class VertexDescriptor
{
    public GPUShaderModule Module { get; set; }
    public string EntryPoint { get; set; }
    public VertexBufferDescriptor[] Buffers { get; set; }
}

public class VertexBufferDescriptor
{
    public int ArrayStride { get; set; }
    public string StepMode { get; set; }
    public VertexAttribute[] Attributes { get; set; }
}

public class VertexAttribute
{
    public int ShaderLocation { get; set; }
    public int Offset { get; set; }
    /// <summary>
    /// Possible Values: https://gpuweb.github.io/gpuweb/#enumdef-gpuvertexformat
    /// </summary>
    public string Format { get; set; }
}

public class CreateBufferDescriptor
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
