using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPUPipelineLayout : IInteropObject, IGPUPipelineLayout
{
    public required JSObject JsObject { get; init; }
}