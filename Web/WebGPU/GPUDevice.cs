using System;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice
/// </summary>
public class GPUDevice : IInteropObject, IGPUDevice
{
    public required JSObject JsObject { get; init; }

    public IGPUQueue Queue
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
    public IGPUCommandEncoder CreateCommandEncoder()
    {
        return new GPUCommandEncoder
        {
            JsObject = Interop.GPUDevice_CreateCommandEncoder(JsObject)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUDevice/createBuffer
    /// </summary>
    public IGPUBuffer CreateBuffer(CreateBufferDescriptor descriptor)
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
    public IGPURenderPipeline CreateRenderPipeline(RenderPipelineDescriptor descriptor)
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
    public IGPUBindGroup CreateBindGroup(BindGroupDescriptor descriptor)
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
    public IGPUBindGroupLayout CreateBindGroupLayout(BindGroupLayoutDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPUBindGroupLayout
        {
            JsObject = Interop.GPUDevice_CreateBindGroupLayout(JsObject, json, references)
        };
    }

    public IGPUShaderModule CreateShaderModule(ShaderModuleDescriptor descriptor)
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
    public IGPUTexture CreateTexture(TextureDescriptor descriptor)
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
    public IGPUPipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor)
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
    public IGPUSampler CreateSampler(SamplerDescriptor descriptor)
    {
        return new GPUSampler
        {
            JsObject = Interop.GPUDevice_CreateSampler(JsObject, InteropHelper.MarshalObj(descriptor))
        };
    }
}