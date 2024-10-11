using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPipeline
/// </summary>
public class GPURenderPipeline : IInteropObject, IGPURenderPipeline
{
    public required JSObject JsObject { get; init; }

    public IGPUBindGroupLayout GetBindGroupLayout(int index)
    {
        return new GPUBindGroupLayout
        {
            JsObject = Interop.GPURenderPipeline_GetBindGroupLayout(JsObject, index)
        };
    }
}
