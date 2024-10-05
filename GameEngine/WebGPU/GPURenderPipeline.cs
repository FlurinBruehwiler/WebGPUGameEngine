using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPipeline
/// </summary>
public class GPURenderPipeline : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public GPUBindGroupLayout GetBindGroupLayout(int index)
    {
        return new GPUBindGroupLayout
        {
            JsObject = Interop.GPURenderPipeline_GetBindGroupLayout(JsObject, index)
        };
    }
}
