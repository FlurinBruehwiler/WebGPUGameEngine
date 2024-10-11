using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

public class GPU
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPU/getPreferredCanvasFormat
    /// </summary>
    public static TextureFormat GetPreferredCanvasFormat()
    {
        var str = Interop.GPU_GetPreferredCanvasFormat();
        if (str == "rgba8unorm")
            return TextureFormat.Rgba8Unorm;
        if (str == "bgra8unorm")
            return TextureFormat.Bgra8Unorm;
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
