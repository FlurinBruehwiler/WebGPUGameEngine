namespace Client.WebGPU;

public interface IGPUQueue
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    void WriteBuffer(IGPUBuffer buffer, int bufferOffset, double[] data, int dataOffset, int size);

    void WriteTexture(TextureDestination destination, byte[] data, DataLayout dataLayout, TextureSize size);

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/submit
    /// </summary>
    void Submit(IGPUCommandBuffer[] commandBuffers);
}

public struct TextureDestination
{
    public required IGPUTexture Texture{ get; set; }
}

public struct DataLayout
{
    public required int BytesPerRow{ get; set; }
    public required int RowsPerImage;
}

public struct TextureSize
{
    public required int Width{ get; set; }
    public required int Height{ get; set; }
}