using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUBindGroup
/// </summary>
public class GPUBindGroup : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
