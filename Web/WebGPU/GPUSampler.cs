using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPUSampler : IInteropObject, IGPUSampler
{
    public required JSObject JsObject { get; init; }
}