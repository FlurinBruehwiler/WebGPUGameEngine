using System;
using System.Threading.Tasks;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPU
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPU/getPreferredCanvasFormat
    /// </summary>
    public static GPUTextureFormat GetPreferredCanvasFormat()
    {
        var str = Interop.GPU_GetPreferredCanvasFormat();
        if (str == "rgba8unorm")
            return GPUTextureFormat.Rgba8Unorm;
        if (str == "bgra8unorm")
            return GPUTextureFormat.Bgra8Unorm;
        throw new Exception("$Unknown texture format {str}");
    }

    public static async Task<GPUAdapter> RequestAdapter()
    {
        return new GPUAdapter
        {
            JsObject = await Interop.GPU_RequestAdapter()
        };
    }
}
