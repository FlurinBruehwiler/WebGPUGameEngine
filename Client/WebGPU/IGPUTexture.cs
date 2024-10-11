namespace GameEngine.WebGPU;

public interface IGPUTexture : IDisposable
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    IGPUTextureView CreateView();

    void Destory();
}