using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder
/// </summary>
public class GPURenderPassEncoder : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public void SetPipeline(GPURenderPipeline renderPipeline)
    {
        Interop.GPURenderPassEncoder_SetPipeline(JsObject, renderPipeline.JsObject);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setVertexBuffer
    /// </summary>
    public void SetVertexBuffer(int slot, GPUBuffer buffer)
    {
        Interop.GPURenderPassEncoder_SetVertexBuffer(JsObject, slot, buffer.JsObject);
    }

    public void SetBindGroup(int slot, GPUBindGroup bindGroup)
    {
        Interop.GPURenderPassEncoder_SetBindGroup(JsObject, slot, bindGroup.JsObject);
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
