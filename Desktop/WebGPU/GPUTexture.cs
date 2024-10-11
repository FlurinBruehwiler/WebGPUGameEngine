using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture
/// </summary>
public unsafe class GPUTexture : IGPUTexture
{
    public required Texture* Texture;

    public IGPUTextureView CreateView()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    public IGPUTextureView CreateView(GPUTextureViewDescriptor textureViewDescriptor)
    {
        TextureViewDescriptor descriptor = new TextureViewDescriptor
        {
            Format = (TextureFormat)textureViewDescriptor.Format,
            Dimension = TextureViewDimension.Dimension2D,
            BaseMipLevel = 0,
            MipLevelCount = 1,
            BaseArrayLayer = 0,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All
        };

        return new GPUTextureView
        {
            TextureView = GPU.API.TextureCreateView(Texture, in descriptor)
        };
    }

    public GPUTextureFormat GetFormat()
    {
        return (GPUTextureFormat)GPU.API.TextureGetFormat(Texture);
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

