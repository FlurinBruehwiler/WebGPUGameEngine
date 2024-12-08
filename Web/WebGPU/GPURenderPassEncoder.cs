using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder
/// </summary>
public class GPURenderPassEncoder : IInteropObject, IGPURenderPassEncoder
{
    public required JSObject JsObject { get; init; }

    public void SetPipeline(IGPURenderPipeline renderPipeline)
    {
        Interop.GPURenderPassEncoder_SetPipeline(JsObject, ((GPURenderPipeline)renderPipeline).JsObject);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setVertexBuffer
    /// </summary>
    public void SetVertexBuffer(int slot, IGPUBuffer buffer)
    {
        Interop.GPURenderPassEncoder_SetVertexBuffer(JsObject, slot, ((GPUBuffer)buffer).JsObject);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setBindGroup
    /// </summary>
    public void SetBindGroup(int index, IGPUBindGroup bindGroup)
    {
        Interop.GPURenderPassEncoder_SetBindGroup(JsObject, index, ((GPUBindGroup)bindGroup).JsObject);
    }

    public void SetBindGroup(int index, IGPUBindGroup bindGroup, uint[] dynamicOffsets, int dynamicOffsetsStart, int dynamicOffsetsLength)
    {
        //todo remove toarray...
        Interop.GPURenderPassEncoder_SetBindGroup(JsObject, index, ((GPUBindGroup)bindGroup).JsObject, dynamicOffsets.Select(x => (int)x).ToArray(), dynamicOffsetsStart, dynamicOffsetsLength);
    }

    public void Draw(int vertexCount)
    {
        Interop.GPURenderPassEncoder_Draw(JsObject, vertexCount);
    }

    public void End()
    {
        Interop.GPURenderPassEncoder_End(JsObject);
    }
}
