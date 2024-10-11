namespace GameEngine.WebGPU;

public interface IGPUCommandEncoder
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/beginRenderPass
    /// </summary>
    IGPURenderPassEncoder BeginRenderPass(GPURenderPassDescriptor descriptor);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUCommandEncoder/finish
    /// </summary>
    IGPUCommandBuffer Finish();
}

public struct GPURenderPassDescriptor
{
    public required ColorAttachment[] ColorAttachments { get; init; }
    public required DepthStencilAttachment DepthStencilAttachment { get; init; }
}

public struct DepthStencilAttachment
{
    public required IGPUTextureView View { get; init; }
    public required GPUStoreOp DepthGpuStoreOp { get; init; }
    public required GPULoadOp DepthGpuLoadOp { get; init; }
    public required float DepthClearValue { get; init; }
}

public enum GPULoadOp
{
    Undefined = 0,
    Clear = 1,
    Load = 2,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public enum GPUStoreOp
{
    Undefined = 0,
    Store = 1,
    Discard = 2,
    Force32 = 2147483647, // 0x7FFFFFFF
}

public struct ColorAttachment
{
    public required ClearColor ClearValue { get; init; }
    public required GPULoadOp LoadOp { get; init; }
    public required GPUStoreOp StoreOp { get; init; }
    public required IGPUTextureView View { get; init; }
}

public struct ClearColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }
}
