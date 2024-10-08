using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue
/// </summary>
public class GPUQueue : IInteropObject
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    public void WriteBuffer(GPUBuffer buffer, int bufferOffset, double[] data, int dataOffset, int size)
    {
        Interop.GPUQueue_WriteBuffer(JsObject, buffer.JsObject, bufferOffset, data, dataOffset, size);
    }

    public void WriteTexture(TextureDestination destination, byte[] data, DataLayout dataLayout, TextureSize size)
    {
        var (destinationJson, destinationReferences) = InteropHelper.MarshalObjWithReferences(destination);
        var dataLayoutJson = InteropHelper.MarshalObj(dataLayout);
        var sizeJson = InteropHelper.MarshalObj(size);

        Interop.GPUQueue_WriteTexture(JsObject, destinationJson, destinationReferences, data, dataLayoutJson, sizeJson);
    }

    public void CopyExternalImageToTexture(ImageSource source, TextureDestination destination, TextureSize copySize)
    {
        var (sourceJson, sourceReferences) = InteropHelper.MarshalObjWithReferences(source);
        var (destinationJson, destinationReferences) = InteropHelper.MarshalObjWithReferences(destination);
        var copySizeJson = InteropHelper.MarshalObj(copySize);

        Interop.GPUQueue_CopyExternalImageToTexture(JsObject, sourceJson, sourceReferences, destinationJson, destinationReferences, copySizeJson);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/submit
    /// </summary>
    public void Submit(GPUCommandBuffer[] commandBuffers)
    {
        Interop.GPUQueue_Submit(JsObject, commandBuffers.Select(x => x.JsObject).ToArray());
    }
}

public struct ImageSource
{
    public required ImageBitmap Source { get; set; }
    public required bool FlipY{ get; set; }
}

public struct TextureDestination
{
    public required GPUTexture Texture{ get; set; }
}

public struct DataLayout
{
    public required int BytesPerRow{ get; set; }
}

public struct TextureSize
{
    public required int Width{ get; set; }
    public required int Height{ get; set; }
}