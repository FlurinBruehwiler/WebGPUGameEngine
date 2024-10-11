using System;
using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTextureView
/// </summary>
public class GPUTextureView : IInteropObject, IGPUTextureView
{
    public required JSObject JsObject { get; init; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
