using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTexture
/// </summary>
public class GPUTexture : IInteropObject
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
}
