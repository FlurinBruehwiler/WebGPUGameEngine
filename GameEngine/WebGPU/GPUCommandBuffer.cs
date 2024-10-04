using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

public class GPUCommandBuffer : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
