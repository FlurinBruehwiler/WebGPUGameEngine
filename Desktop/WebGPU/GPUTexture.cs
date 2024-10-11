using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture
/// </summary>
public unsafe class GPUTexture : IGPUTexture
{
    public required Texture* Texture;

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    public IGPUTextureView CreateView()
    {
        var descriptor = new TextureViewDescriptor();

        return new GPUTextureView
        {
            TextureView = GPU.API.TextureCreateView(Texture, in descriptor)
        };
    }

    public void Destory()
    {
        GPU.API.TextureDestroy(Texture);
    }

    public void Dispose()
    {
        Destory();
    }
}
