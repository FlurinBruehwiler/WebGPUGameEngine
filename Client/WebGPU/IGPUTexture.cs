namespace Client.WebGPU;

public interface IGPUTexture : IDisposable
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    IGPUTextureView CreateView();
    IGPUTextureView CreateView(GPUTextureViewDescriptor textureViewDescriptor);

    void Destory();
}

public struct GPUTextureViewDescriptor
{
    public GPUTextureFormat Format;
}