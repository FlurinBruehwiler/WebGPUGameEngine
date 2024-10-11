using System;
using System.Threading.Tasks;

namespace Web.WebGPU;

public static class JsWindow
{
    public static string Location => Interop.Window_LocationHref();

    public static void RequestAnimationFrame(Action callback)
    {
        Interop.Window_RequestAnimationFrame(callback);
    }

    public static async Task<ImageBitmap> CreateImageBitmap(byte[] image, BitmapOptions options)
    {
        var imageBitmap = await Interop.Window_CreateImageBitmap(image, InteropHelper.MarshalObj(options));
        return new ImageBitmap
        {
            JsObject = imageBitmap
        };
    }
}

public struct BitmapOptions
{
    public required string ColorSpaceConversion{ get; set; }
}