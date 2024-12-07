using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUCommandEncoder : IGPUCommandEncoder
{
    public required CommandEncoder* CommandEncoder;

    public IGPURenderPassEncoder BeginRenderPass(GPURenderPassDescriptor descriptor)
    {
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
            ColorAttachments = ca,
            ColorAttachmentCount = (nuint)descriptor.ColorAttachments.Length
        };

        if (descriptor.DepthStencilAttachment is {} depthStencilAttachment)
        {
            var dsa = new RenderPassDepthStencilAttachment
            {
                View = ((GPUTextureView)depthStencilAttachment.View).TextureView,
                DepthStoreOp = (StoreOp)depthStencilAttachment.DepthGpuStoreOp,
                DepthLoadOp = (LoadOp)depthStencilAttachment.DepthGpuLoadOp,
                DepthClearValue = depthStencilAttachment.DepthClearValue,
            };
            renderPassDescriptor.DepthStencilAttachment = &dsa;
        }

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