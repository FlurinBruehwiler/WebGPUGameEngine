using System;
using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

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
        var (json, references) = InteropHelper.MarshalComplexObject(descriptor);

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
        var (json, references) = InteropHelper.MarshalComplexObject(descriptor);

        return new GPURenderPipeline
        {
            JsObject = Interop.GPUDevice_CreateRenderPipeline(JsObject, json, references)
        };
    }

    //https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBindGroup
    public GPUBindGroup CreateBindGroup(BindGroupDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalComplexObject(descriptor);

        return new GPUBindGroup
        {
            JsObject = Interop.GPUDevice_CreateBindGroup(JsObject, json, references)
        };
    }

    public GPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalComplexObject(descriptor);

        return new GPUShaderModule
        {
            JsObject = Interop.GPUDevice_CreateShaderModule(JsObject, json, references)
        };
    }
}

public class BindGroupDescriptor
{
    public required GPUBindGroupLayout Layout { get; set; }
    public required BindGroupEntry[] Entries { get; set; }
}

public class BindGroupEntry
{
    public required int Binding { get; init; }
    public required EntryResource Resource { get; init; }
}

public class EntryResource
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
    public required string Layout { get; set; }
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
