using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUBindGroupLayout : IInteropObject, IGPUBindGroupLayout
{
    public required JSObject JsObject { get; init; }
}
