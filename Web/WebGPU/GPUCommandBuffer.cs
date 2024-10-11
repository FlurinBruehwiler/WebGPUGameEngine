using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPUCommandBuffer : IInteropObject, IGPUCommandBuffer
{
    public required JSObject JsObject { get; init; }
}
