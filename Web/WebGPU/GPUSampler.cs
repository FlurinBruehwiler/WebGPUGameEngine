using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUSampler : IInteropObject, IGPUSampler
{
    public required JSObject JsObject { get; init; }
}