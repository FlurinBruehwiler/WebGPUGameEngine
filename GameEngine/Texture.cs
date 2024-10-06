using GameEngine.WebGPU;

namespace GameEngine;

public class Texture : IDisposable
{
    public required GPUTexture GpuTexture;

    public void Dispose()
    {
        GpuTexture.Dispose();
    }
}