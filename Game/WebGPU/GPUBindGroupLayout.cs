using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

public class GPUBindGroupLayout : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
