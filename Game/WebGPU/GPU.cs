namespace Game.WebGPU;

public class GPU
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPU/getPreferredCanvasFormat
    /// </summary>
    public static string GetPreferredCanvasFormat()
    {
        return Interop.GPU_GetPreferredCanvasFormat();
    }

    public static async Task<GPUAdapter> RequestAdapter()
    {
        return new GPUAdapter
        {
            JsObject = await Interop.GPU_RequestAdapter()
        };
    }
}
