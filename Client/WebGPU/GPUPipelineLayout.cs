using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public class GPUPipelineLayout : IInteropObject
{
    public required JSObject JsObject { get; init; }
}