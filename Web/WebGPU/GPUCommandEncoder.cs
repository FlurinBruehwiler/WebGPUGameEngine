using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder
/// </summary>
public class GPUCommandEncoder : IInteropObject, IGPUCommandEncoder
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/beginRenderPass
    /// </summary>
    public IGPURenderPassEncoder BeginRenderPass(RenderPassDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

        return new GPURenderPassEncoder
        {
            JsObject = Interop.GPUCommandEncoder_BeginRenderPass(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/finish
    /// </summary>
    public IGPUCommandBuffer Finish()
    {
        return new GPUCommandBuffer
        {
            JsObject = Interop.GPUCommandEncoder_Finish(JsObject)
        };
    }
}