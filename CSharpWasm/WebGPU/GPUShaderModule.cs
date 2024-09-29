using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

public class GPUShaderModule : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
