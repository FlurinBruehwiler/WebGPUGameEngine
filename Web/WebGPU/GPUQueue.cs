using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Client.WebGPU;

namespace Web.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue
/// </summary>
public class GPUQueue : IInteropObject, IGPUQueue
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    public void WriteBuffer(IGPUBuffer buffer, int bufferOffset, double[] data, int dataOffset, int size)
    {
        Interop.GPUQueue_WriteBuffer(JsObject, ((GPUBuffer)buffer).JsObject, bufferOffset, data, dataOffset, size);
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
    public void Submit(IGPUCommandBuffer[] commandBuffers)
    {
        Interop.GPUQueue_Submit(JsObject, commandBuffers.Select(x => ((GPUCommandBuffer)x).JsObject).ToArray());
    }
}

public struct ImageSource
{
    public required ImageBitmap Source { get; set; }
    public required bool FlipY{ get; set; }
}
