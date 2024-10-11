using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTextureView
/// </summary>
public unsafe class GPUTextureView :  IGPUTextureView
{
    public required TextureView* TextureView;
}
