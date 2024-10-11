using System.Reflection.Metadata.Ecma335;
using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue
/// </summary>
public unsafe class GPUQueue : IGPUQueue
{
    public required Queue* Queue;

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    public void WriteBuffer(IGPUBuffer buffer, int bufferOffset, double[] data, int dataOffset, int size)
    {
        fixed (double* dataPtr = data.AsSpan(dataOffset))
        {
            GPU.API.QueueWriteBuffer(Queue, ((GPUBuffer)buffer).Buffer, (uint)bufferOffset, dataPtr, (nuint)size);
        }
    }

    public void WriteTexture(TextureDestination destination, byte[] data, DataLayout dataLayout, TextureSize size)
    {
        var imageCopyTexture = new ImageCopyTexture
        {
            Texture = ((GPUTexture)destination.Texture).Texture
        };

        var textureDataLayout = new TextureDataLayout
        {
            BytesPerRow = (uint)dataLayout.BytesPerRow
        };

        var extend = new Extent3D
        {
            Height = (uint)size.Height,
            Width = (uint)size.Width
        };

        fixed (byte* dataPtr = data)
        {
            GPU.API.QueueWriteTexture(Queue, in imageCopyTexture, dataPtr, (nuint)data.Length, in textureDataLayout, in extend);
        }
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/submit
    /// </summary>
    public void Submit(IGPUCommandBuffer[] commandBuffers)
    {
        var buffers = stackalloc CommandBuffer*[commandBuffers.Length];
        for (var i = 0; i < commandBuffers.Length; i++)
        {
            buffers[i] = ((GPUCommandBuffer)commandBuffers[i]).CommandBuffer;
        }

        GPU.API.QueueSubmit(Queue, (nuint)commandBuffers.Length, buffers);
    }
}