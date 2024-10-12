global using NativeGPU = Silk.NET.WebGPU.WebGPU;
using Client;
using Client.WebGPU;
using Desktop.WebGPU;
using Silk.NET.GLFW;
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

        var textureFormat = surface.GetPreferredFormat(adapter);

        surface.Configure(new GPUSurfaceConfiguration
        {
            Width = 1000,
            Height = 600,
            TextureFormat = textureFormat,
            Usage = GPUTextureUsage.RENDER_ATTACHMENT,
            Device = (GPUDevice)device,
            PresentMode = PresentMode.Fifo,
            AlphaMode = CompositeAlphaMode.Auto
        });

        var desktopImpl = new DesktopImplementation(surface);

        Game.GameInfo = Game.InitializeGame(desktopImpl, device, textureFormat).GetAwaiter().GetResult();
        SetupInput(window);

        while (!window.Glfw.WindowShouldClose(window.GetHandle()))
        {
            UpdateMouseDeltas(window);

            Game.Frame();

            surface.Present();
            window.Glfw.PollEvents();
        }
    }

    private static double lastMousePosX;
    private static double lastMousePosY;

    private static unsafe void UpdateMouseDeltas(Window window)
    {
        window.Glfw.GetCursorPos(window.GetHandle(), out var xPos, out var yPos);

        Game.GameInfo.Input.MouseChangeX = (float)(xPos - lastMousePosX);
        Game.GameInfo.Input.MouseChangeY = (float)(yPos - lastMousePosY);

        lastMousePosX = xPos;
        lastMousePosY = yPos;
    }

    private static unsafe void SetupInput(Window window)
    {
        window.Glfw.SetInputMode(window.GetHandle(), CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);

        if (!window.Glfw.RawMouseMotionSupported())
        {
            throw new Exception("Raw Mouse Motion not supported!");
        }

        window.Glfw.SetInputMode(window.GetHandle(), CursorStateAttribute.RawMouseMotion, true);

        window.Glfw.SetKeyCallback(window.GetHandle(), KeyCallback);
    }

    private static unsafe void KeyCallback(WindowHandle* window, Keys key, int scancode, InputAction action, KeyModifiers mods)
    {
        var inputSystem = Game.GameInfo.Input;

        if (action == InputAction.Press)
        {
            inputSystem.InformKeyChanged((KeyboardKeys)key, true);
        }
        else if(action == InputAction.Release)
        {
            inputSystem.InformKeyChanged((KeyboardKeys)key, false);
        }
    }
}