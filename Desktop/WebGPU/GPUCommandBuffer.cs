using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUCommandBuffer :  IGPUCommandBuffer
{
    public required CommandBuffer* CommandBuffer;
}
