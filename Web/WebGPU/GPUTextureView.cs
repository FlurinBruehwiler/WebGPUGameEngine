using System.Runtime.InteropServices.JavaScript;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTextureView
/// </summary>
public class GPUTextureView : IInteropObject, IGPUTextureView
{
    public required JSObject JsObject { get; init; }
}
