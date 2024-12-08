namespace Client.WebGPU;

public interface IGPURenderPassEncoder
{
    void SetPipeline(IGPURenderPipeline renderPipeline);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setVertexBuffer
    /// </summary>
    void SetVertexBuffer(int slot, IGPUBuffer buffer);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/setBindGroup
    /// </summary>
    void SetBindGroup(int index, IGPUBindGroup bindGroup);

    void SetBindGroup(int index, IGPUBindGroup bindGroup, uint[] dynamicOffsets, int dynamicOffsetsStart, int dynamicOffsetsLength);
    void Draw(int vertexCount);
    void End();
}