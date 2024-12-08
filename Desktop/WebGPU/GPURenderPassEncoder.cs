using Client.WebGPU;
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
        GPU.API.RenderPassEncoderSetPipeline(RenderPassEncoder, ((GPURenderPipeline)renderPipeline).RenderPipeline);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setVertexBuffer
    /// </summary>
    public void SetVertexBuffer(int slot, IGPUBuffer buffer)
    {
        GPU.API.RenderPassEncoderSetVertexBuffer(RenderPassEncoder, (uint)slot, ((GPUBuffer)buffer).Buffer, 0, (ulong)((GPUBuffer)buffer).Size);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setBindGroup
    /// </summary>
    public void SetBindGroup(int index, IGPUBindGroup bindGroup)
    {
        uint dynamicOffsets = 0;

        GPU.API.RenderPassEncoderSetBindGroup(RenderPassEncoder, (uint)index, ((GPUBindGroup)bindGroup).BindGroup, 0, in dynamicOffsets);
    }

    public void SetBindGroup(int index, IGPUBindGroup bindGroup, uint[] dynamicOffsets, int dynamicOffsetsStart, int dynamicOffsetsLength)
    {
        var span = dynamicOffsets.AsSpan(dynamicOffsetsStart, dynamicOffsetsLength);
        GPU.API.RenderPassEncoderSetBindGroup(RenderPassEncoder, (uint)index, ((GPUBindGroup)bindGroup).BindGroup,
            (UIntPtr)dynamicOffsetsLength, span);
    }

    public void Draw(int vertexCount)
    {
        GPU.API.RenderPassEncoderDraw(RenderPassEncoder, (uint)vertexCount, 1, 0, 0);
    }

    public void End()
    {
        GPU.API.RenderPassEncoderEnd(RenderPassEncoder);
    }
}
