using System.Runtime.InteropServices.JavaScript;

namespace Web.WebGPU;

public class ImageBitmap : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public int Width => JsObject.GetPropertyAsInt32("width");
    public int Height => JsObject.GetPropertyAsInt32("height");
}