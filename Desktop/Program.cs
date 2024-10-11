// See https://aka.ms/new-console-template for more information

global using NativeGPU = Silk.NET.WebGPU.WebGPU;
using Client.WebGPU;
using Desktop.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop;

public static class Program
{
    public static unsafe void Main()
    {
        var window = Window.Initialize();

        if(window == null)
            return;

        var surface = window.CreateSurface();

        var adapter = GPU.RequestAdapter(new GPURequestAdapterOptions
        {
            CompatibleSurface = surface
        }).GetAwaiter().GetResult();

        var device = adapter.RequestDevice().GetAwaiter().GetResult();

        surface.Configure(new GPUSurfaceConfiguration
        {
            Width = 1000,
            Height = 600,
            TextureFormat = surface.GetPreferredFormat(adapter),
            Usage = GPUTextureUsage.RENDER_ATTACHMENT,
            Device = (GPUDevice)device,
            PresentMode = PresentMode.Fifo,
            AlphaMode = CompositeAlphaMode.Auto
        });

        var desktopImpl = new DesktopImplementation(surface);

        while (!window.Glfw.WindowShouldClose(window.GetHandle()))
        {
            window.Glfw.PollEvents();

            surface.Present();
        }
    }

}