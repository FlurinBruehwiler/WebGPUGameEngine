namespace GameEngine.WebGPU;

public interface IGPUCommandEncoder
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/beginRenderPass
    /// </summary>
    IGPURenderPassEncoder BeginRenderPass(RenderPassDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/finish
    /// </summary>
    IGPUCommandBuffer Finish();
}

public class RenderPassDescriptor
{
    public required ColorAttachment[] ColorAttachments { get; init; }
    public required DepthStencilAttachment DepthStencilAttachment { get; init; }
}

public class DepthStencilAttachment
{
    public required IGPUTextureView View { get; init; }
    public required string DepthStoreOp { get; init; }
    public required string DepthLoadOp { get; init; }
    public required float DepthClearValue { get; init; }
}

public class ColorAttachment
{
    public required ClearColor ClearValue { get; init; }
    public required string LoadOp { get; init; }
    public required string StoreOp { get; init; }
    public required IGPUTextureView View { get; init; }
}

public class ClearColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }
}
