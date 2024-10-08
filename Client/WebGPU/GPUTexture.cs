using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture
/// </summary>
public class GPUTexture : IInteropObject, IDisposable
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    public GPUTextureView CreateView()
    {
        return new GPUTextureView
        {
            JsObject = Interop.GPUTexture_CreateView(JsObject)
        };
    }

    public void Destory()
    {
        Interop.GPUTexture_Destroy(JsObject);
    }

    public void Dispose()
    {
        Destory();
    }
}
