using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GameEngine;
using GameEngine.WebGPU;
using WasmTestCSharp.WebGPU;

namespace WasmTestCSharp;

public class WebPlatformImplementation(GPUCanvasContext canvasContext) : IPlatformImplementation
{
    public static HttpClient HttpClient = new();

    public async Task<Stream> LoadStream(string name)
    {
        string url = Path.Combine(JsWindow.Location, name);

        return await HttpClient.GetStreamAsync(url);
    }

    public IGPUTextureView CreateView()
    {
        return canvasContext.GetCurrentTexture().CreateView();
    }

    public async Task<Texture> LoadTexture(string name)
    {
        var stream = await LoadStream(name);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var imageBitmap = await JsWindow.CreateImageBitmap(ms.ToArray(), new BitmapOptions
        {
            ColorSpaceConversion = "none"
        });
        var gpuTexture = Helper.CreateTextureFromBitmap(imageBitmap);
        return new Texture
        {
            GpuTexture = gpuTexture
        };
    }
}