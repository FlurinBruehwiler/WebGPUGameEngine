using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

public class GPUBindGroupLayout : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
