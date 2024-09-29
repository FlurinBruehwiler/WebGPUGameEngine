using System;
using System.Threading.Tasks;

namespace WasmTestCSharp.WebGPU;

public class GPU
{
    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPU/getPreferredCanvasFormat
    /// </summary>
    public static TextureFormat GetPreferredCanvasFormat()
    {
        var strValue = Interop.GPU_GetPreferredCanvasFormat();

        if (Enum.TryParse<TextureFormat>(strValue, out var textureFormat))
        {
            return textureFormat;
        }

        throw new Exception($"Invalid texture format: {strValue}");
    }

    public static async Task<GPUAdapter> RequestAdapter()
    {
        return new GPUAdapter
        {
            JsObject = await Interop.GPU_RequestAdapter()
        };
    }
}
