namespace GameEngine.WebGPU;

public static class JsWindow
{
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
    public required string ClorSpaceConversion{ get; set; }
}