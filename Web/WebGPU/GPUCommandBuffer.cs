using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

public class GPUCommandBuffer : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
