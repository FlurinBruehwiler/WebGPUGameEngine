using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUTextureView
/// </summary>
public class GPUTextureView : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
