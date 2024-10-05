using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public class GPUShaderModule : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
