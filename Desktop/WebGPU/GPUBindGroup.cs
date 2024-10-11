using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBindGroup
/// </summary>
public unsafe class GPUBindGroup : IGPUBindGroup
{
    public required BindGroup* BindGroup;
}
