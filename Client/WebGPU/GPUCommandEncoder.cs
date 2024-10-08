using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

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
        var (json, references) = InteropHelper.MarshalObjWithReferences(descriptor);

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
    public required ColorAttachment[] ColorAttachments { get; init; }
    public required DepthStencilAttachment DepthStencilAttachment { get; init; }
}

public class DepthStencilAttachment
{
    public required GPUTextureView View { get; init; }
    public required string DepthStoreOp { get; init; }
    public required string DepthLoadOp { get; init; }
    public required float DepthClearValue { get; init; }
}

public class ColorAttachment
{
    public required ClearColor ClearValue { get; init; }
    public required string LoadOp { get; init; }
    public required string StoreOp { get; init; }
    public required GPUTextureView View { get; init; }
}

public class ClearColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }
}
