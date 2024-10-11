using GameEngine.WebGPU;
using Silk.NET.WebGPU;
using WasmTestCSharp.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue
/// </summary>
public unsafe class GPUQueue : IGPUQueue
{
    public required Queue* Queue;

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    public void WriteBuffer(IGPUBuffer buffer, int bufferOffset, double[] data, int dataOffset, int size)
    {
    }

    public void WriteTexture(TextureDestination destination, byte[] data, DataLayout dataLayout, TextureSize size)
    {
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/submit
    /// </summary>
    public void Submit(IGPUCommandBuffer[] commandBuffers)
    {
    }
}