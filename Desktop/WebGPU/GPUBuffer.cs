using GameEngine.WebGPU;
using Buffer = Silk.NET.WebGPU.Buffer;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBuffer
/// </summary>
public unsafe class GPUBuffer : IGPUBuffer
{
    public required Buffer* Buffer;

    public void Destory()
    {
    }

    public void Dispose()
    {
        Destory();
    }
}
