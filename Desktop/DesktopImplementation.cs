using Client;
using Client.WebGPU;
using Desktop.WebGPU;

namespace Desktop;

public class DesktopImplementation(GPUSurface surface) : IPlatformImplementation
{
    public Task<Texture> LoadTexture(string name)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> LoadStream(string name)
    {
        throw new NotImplementedException();
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