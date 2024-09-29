using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBuffer
/// </summary>
public class GPUBuffer : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public void Destory()
    {
        Interop.GPUBuffer_Destroy(JsObject);
    }
}
