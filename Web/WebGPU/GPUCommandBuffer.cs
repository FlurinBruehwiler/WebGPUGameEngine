using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUCommandBuffer : IInteropObject, IGPUCommandBuffer
{
    public required JSObject JsObject { get; init; }
}
