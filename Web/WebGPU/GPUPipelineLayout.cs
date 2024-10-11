using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUPipelineLayout : IInteropObject, IGPUPipelineLayout
{
    public required JSObject JsObject { get; init; }
}