using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public class GPUSampler : IInteropObject, IBindGroupResource
{
    public required JSObject JsObject { get; init; }
}