﻿using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

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
        var (json, references) = InteropHelper.MarshalObjWithReferences(config);

        Interop.GPUCanvasContext_Configure(JsObject, json, references);
    }
}

public struct ContextConfig
{
    public required IGPUDevice Device { get; set; }
    public required GPUTextureFormat Format { get; set; }
    public required  string AlphaMode { get; set; }
}
