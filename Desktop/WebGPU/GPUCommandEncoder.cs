using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUCommandEncoder : IGPUCommandEncoder
{
    public required CommandEncoder* CommandEncoder;

    public IGPURenderPassEncoder BeginRenderPass(GPURenderPassDescriptor descriptor)
    {
        var dsa = new RenderPassDepthStencilAttachment
        {
            View = ((GPUTextureView)descriptor.DepthStencilAttachment.View).TextureView,
            DepthStoreOp = (StoreOp)descriptor.DepthStencilAttachment.DepthGpuStoreOp,
            DepthLoadOp = (LoadOp)descriptor.DepthStencilAttachment.DepthGpuLoadOp,
            DepthClearValue = descriptor.DepthStencilAttachment.DepthClearValue,
        };

        var ca = stackalloc RenderPassColorAttachment[descriptor.ColorAttachments.Length];
        for (var i = 0; i < descriptor.ColorAttachments.Length; i++)
        {
            var x = descriptor.ColorAttachments[i];
            ca[i] = new RenderPassColorAttachment
            {
                View = ((GPUTextureView)x.View).TextureView,
                StoreOp = (StoreOp)x.StoreOp,
                LoadOp = (LoadOp)x.LoadOp,
                ClearValue = new Color(x.ClearValue.R, x.ClearValue.G, x.ClearValue.B, x.ClearValue.A)
            };
        }

        var renderPassDescriptor = new RenderPassDescriptor
        {
            DepthStencilAttachment = &dsa,
            ColorAttachments = ca,
            ColorAttachmentCount = (nuint)descriptor.ColorAttachments.Length
        };

        return new GPURenderPassEncoder
        {
            RenderPassEncoder = GPU.API.CommandEncoderBeginRenderPass(CommandEncoder, in renderPassDescriptor)
        };
    }

    public IGPUCommandBuffer Finish()
    {
        var descriptor = new CommandBufferDescriptor();

        return new GPUCommandBuffer
        {
            CommandBuffer = GPU.API.CommandEncoderFinish(CommandEncoder, in descriptor)
        };
    }
}