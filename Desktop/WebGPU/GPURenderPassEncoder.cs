using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder
/// </summary>
public unsafe class GPURenderPassEncoder : IGPURenderPassEncoder
{
    public required RenderPassEncoder* RenderPassEncoder { get; init; }

    public void SetPipeline(IGPURenderPipeline renderPipeline)
    {
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setVertexBuffer
    /// </summary>
    public void SetVertexBuffer(int slot, IGPUBuffer buffer)
    {
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setBindGroup
    /// </summary>
    public void SetBindGroup(int index, IGPUBindGroup bindGroup)
    {
    }

    public void SetBindGroup(int index, IGPUBindGroup bindGroup, int[] dynamicOffsets, int dynamicOffsetsStart, int dynamicOffsetsLength)
    {
    }

    public void Draw(int vertexCount)
    {
    }

    public void End()
    {
    }
}
