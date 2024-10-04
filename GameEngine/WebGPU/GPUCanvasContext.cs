using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUCanvasContext
/// </summary>
public class GPUCanvasContext : IInteropObject
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCanvasContext/getCurrentTexture
    /// </summary>
    public GPUTexture GetCurrentTexture()
    {
        return new GPUTexture
        {
            JsObject = Interop.GPUCanvasContext_GetCurrentTexture(JsObject)
        };
    }

    public void Configure(ContextConfig config)
    {
        var (json, references) = InteropHelper.MarshalComplexObject(config);

        Interop.GPUCanvasContext_Configure(JsObject, json, references);
    }
}

public class ContextConfig
{
    public GPUDevice Device { get; set; }
    public string Format { get; set; }
    public string AlphaMode { get; set; }
}
