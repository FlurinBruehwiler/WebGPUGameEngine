using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public class GPUBindGroupLayout : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
