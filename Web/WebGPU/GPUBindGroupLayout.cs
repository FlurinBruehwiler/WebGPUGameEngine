using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPUBindGroupLayout : IInteropObject, IGPUBindGroupLayout
{
    public required JSObject JsObject { get; init; }
}
