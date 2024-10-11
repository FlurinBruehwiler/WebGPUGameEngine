using GameEngine.WebGPU;

namespace GameEngine;

public class Texture : IDisposable
{
    public required IGPUTexture GpuTexture;

    public void Dispose()
    {
        GpuTexture.Dispose();
    }
}