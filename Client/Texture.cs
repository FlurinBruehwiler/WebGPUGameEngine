using Client.WebGPU;

namespace Client;

public class Texture : IDisposable
{
    public required IGPUTexture GpuTexture;

    public void Dispose()
    {
        GpuTexture.Dispose();
    }
}