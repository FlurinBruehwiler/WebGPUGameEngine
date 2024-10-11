using GameEngine.WebGPU;
using Silk.NET.WebGPU;
using RenderPassDescriptor = GameEngine.WebGPU.RenderPassDescriptor;

namespace Desktop.WebGPU;

public unsafe class GPUCommandEncoder : IGPUCommandEncoder
{
    public required CommandEncoder* CommandEncoder;

    public IGPURenderPassEncoder BeginRenderPass(RenderPassDescriptor descriptor)
    {
        throw new NotImplementedException();
    }

    public IGPUCommandBuffer Finish()
    {
        throw new NotImplementedException();
    }
}