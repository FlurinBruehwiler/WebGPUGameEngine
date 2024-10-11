using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPUShaderModule : IInteropObject, IGPUShaderModule
{
    public required JSObject JsObject { get; init; }
}
