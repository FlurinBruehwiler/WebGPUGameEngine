using System.Drawing;
using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder
/// </summary>
public class GPUCommandEncoder : IInteropObject
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/beginRenderPass
    /// </summary>
    public GPURenderPassEncoder BeginRenderPass(RenderPassDescriptor descriptor)
    {
        var (json, references) = InteropHelper.MarshalComplexObject(descriptor);

        return new GPURenderPassEncoder
        {
            JsObject = Interop.GPUCommandEncoder_BeginRenderPass(JsObject, json, references)
        };
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/finish
    /// </summary>
    public GPUCommandBuffer Finish()
    {
        return new GPUCommandBuffer
        {
            JsObject = Interop.GPUCommandEncoder_Finish(JsObject)
        };
    }
}

public class RenderPassDescriptor
{
    public required ColorAttachement[] ColorAttachements { get; init; }
}

public class ColorAttachement
{
    public Color ClearColor { get; init; }
    public required LoadOp LoadOp { get; init; }
    public required StoreOp StoreOp { get; init; }
    public required GPUTextureView View { get; init; }
}

public enum LoadOp
{
    Clear,
    Load
}

public enum StoreOp
{
    Discard,
    Store
}
