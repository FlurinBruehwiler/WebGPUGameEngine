using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBuffer
/// </summary>
public class GPUBuffer : IInteropObject, IGPUBuffer
{
    public required JSObject JsObject { get; init; }

    public void Destory()
    {
        Interop.GPUBuffer_Destroy(JsObject);
    }

    public void Dispose()
    {
        Destory();
    }
}
