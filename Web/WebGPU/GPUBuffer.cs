using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

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
