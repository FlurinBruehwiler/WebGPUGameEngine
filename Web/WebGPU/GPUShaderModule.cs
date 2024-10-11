using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUShaderModule : IInteropObject, IGPUShaderModule
{
    public required JSObject JsObject { get; init; }
}
