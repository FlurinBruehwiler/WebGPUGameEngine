using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture
/// </summary>
public class GPUTexture : IInteropObject, IGPUTexture
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture/createView
    /// </summary>
    public IGPUTextureView CreateView()
    {
        return new GPUTextureView
        {
            JsObject = Interop.GPUTexture_CreateView(JsObject)
        };
    }

    public IGPUTextureView CreateView(GPUTextureViewDescriptor textureViewDescriptor)
    {
        throw new System.NotImplementedException();
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
