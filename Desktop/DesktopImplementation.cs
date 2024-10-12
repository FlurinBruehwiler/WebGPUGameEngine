using Client;
using Client.WebGPU;
using Desktop.WebGPU;
using SkiaSharp;

namespace Desktop;

public class DesktopImplementation(GPUSurface surface) : IPlatformImplementation
{
    public async Task<Texture> LoadTexture(string name)
    {
        var textureStream = await LoadStream(name);

        var image = SKImage.FromEncodedData(textureStream);
        var bitmap = SKBitmap.FromImage(image);

        var texture = Game.GameInfo.Device.CreateTexture(new TextureDescriptor
        {
            Format = GPUTextureFormat.Rgba8Unorm,
            Usage = GPUTextureUsage.TEXTURE_BINDING |
                    GPUTextureUsage.COPY_DST |
                    GPUTextureUsage.RENDER_ATTACHMENT,
            Size = new SizeObject
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                DepthOrArrayLayers = 1
            }
        });

        Game.GameInfo.Device.Queue.WriteTexture(new TextureDestination
            {
                Texture = texture
            },
            bitmap.Bytes,
            new DataLayout
            {
                BytesPerRow = bitmap.Width * 4
            },
            new TextureSize
            {
                Width = bitmap.Width,
                Height = bitmap.Height
            });

        return new Texture
        {
            GpuTexture = texture
        };
    }

    public Task<Stream> LoadStream(string name)
    {
        Stream stream = File.OpenRead(@"C:\Programming\Github\webgputest\Web\wwwroot\" + name);
        return Task.FromResult(stream);
    }

    public IGPUTextureView CreateTextureView()
    {
        var texture = surface.GetCurrentTexture();
        return texture.CreateView(new GPUTextureViewDescriptor
        {
            Format = texture.GetFormat()
        });
    }
}