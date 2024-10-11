using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBindGroup
/// </summary>
public class GPUBindGroup : IInteropObject, IGPUBindGroup
{
    public required JSObject JsObject { get; init; }
}
