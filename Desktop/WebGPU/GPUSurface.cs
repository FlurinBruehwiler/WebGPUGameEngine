using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUSurface
{
    public required Surface* Surface;

    public void Configure(GPUSurfaceConfiguration surfaceConfiguration)
    {
        var config = new SurfaceConfiguration
        {
            Width = (uint)surfaceConfiguration.Width,
            Height = (uint)surfaceConfiguration.Height,
            Format = (TextureFormat)surfaceConfiguration.TextureFormat,
            Usage = (TextureUsage)surfaceConfiguration.Usage,
            Device = surfaceConfiguration.Device.Device,
            PresentMode = surfaceConfiguration.PresentMode,
            AlphaMode = surfaceConfiguration.AlphaMode
        };

        GPU.API.SurfaceConfigure(Surface, in config);
    }

    public GPUTextureFormat GetPreferredFormat(GPUAdapter adapter)
    {
        return (GPUTextureFormat)GPU.API.SurfaceGetPreferredFormat(Surface, adapter.Adapter);
    }

    public GPUTexture GetCurrentTexture()
    {
        var surfaceTexture = new SurfaceTexture();

        GPU.API.SurfaceGetCurrentTexture(Surface, ref surfaceTexture);

        if (surfaceTexture.Status != SurfaceGetCurrentTextureStatus.Success)
        {
            throw new Exception($"Error creating surface {surfaceTexture.Status}");
        }

        if (surfaceTexture.Suboptimal)
        {
            Console.WriteLine("Texture is suboptimal");
        }

        return new GPUTexture
        {
            Texture = surfaceTexture.Texture
        };
    }

    public void Present()
    {
        GPU.API.SurfacePresent(Surface);
    }
}

public struct GPUSurfaceConfiguration
{
    public int Width;
    public int Height;
    public GPUTextureFormat TextureFormat;
    public GPUTextureUsage Usage;
    public GPUDevice Device;
    public PresentMode PresentMode;
    public CompositeAlphaMode AlphaMode;
}