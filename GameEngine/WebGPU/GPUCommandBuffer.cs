using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public class GPUCommandBuffer : IInteropObject
{
    public required JSObject JsObject { get; init; }
}
