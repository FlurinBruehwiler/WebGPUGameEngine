using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUBindGroupLayout : IGPUBindGroupLayout
{
    public required BindGroupLayout* BindGroupLayout;
}
