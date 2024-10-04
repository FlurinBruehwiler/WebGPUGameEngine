using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

public class GPUShaderModule : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
