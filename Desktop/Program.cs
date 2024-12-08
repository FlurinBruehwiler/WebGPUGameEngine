global using NativeGPU = Silk.NET.WebGPU.WebGPU;
using System.Diagnostics;
using System.Numerics;
using Client;
using Client.WebGPU;
using Desktop.WebGPU;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Color = System.Drawing.Color;

namespace Desktop;

public static class Program
{
    private static GPUSurface _surface;
    private static GPUDevice _device;
    private static IWindow _window;

    static unsafe void onLoad(IWindow window)
    {
        _surface = new GPUSurface
        {
            Surface = window.CreateWebGPUSurface(GPU.API, GPU.Instance)
        };

        var adapter = GPU.RequestAdapter(new GPURequestAdapterOptions
        {
            CompatibleSurface = _surface,
        }).GetAwaiter().GetResult();

        _device = (GPUDevice)adapter.RequestDevice().GetAwaiter().GetResult();

        var textureFormat = _surface.GetPreferredFormat(adapter);

        var desktopImpl = new DesktopImplementation(_surface);

        GPU.API.DeviceSetUncapturedErrorCallback(_device.Device, new PfnErrorCallback((errorType, message, data) =>
        {
            Console.WriteLine($"{errorType}: {SilkMarshal.PtrToString((nint) message)}");
        }), null);

        Game.GameInfo = Game.InitializeGame(desktopImpl, _device, textureFormat).GetAwaiter().GetResult();
        Game.GameInfo.ScreenWidth = 800;
        Game.GameInfo.ScreenHeight = 600;

        Game.GameInfo.NullTexture = Game.GameInfo.ResourceManager.LoadTexture("NullTexture.png").GetAwaiter().GetResult();
        Game.GameInfo.Camera = new Camera
        {
            Transform = new Transform
            {
                Position = new Vector3(10, 0, 0),
                Scale = Vector3.One,
                Rotation = new Vector3(0, MathF.PI / 2, 0),
            }
        };

        var cubeModel = Game.GameInfo.ResourceManager.LoadModel("crate.obj").GetAwaiter().GetResult();
        Renderer.UploadModel(cubeModel);
        cubeModel.SolidColor = System.Drawing.Color.Beige;;

        // cubeModel.Texture = await ResourceManager.LoadTexture("crate-texture.jpg");

        for (int i = 0; i < 10; i++)
        {
            Game.GameInfo.Entities.Add(new Entity
            {
                Transform = new Transform
                {
                    Scale = Vector3.One,
                    Position = Game.RandomVector(-25, 25) with { Y = 1 }
                },
                Model = cubeModel,
            });
        }

        var planeModel = Game.GameInfo.ResourceManager.LoadModel("plane.obj").GetAwaiter().GetResult();
        Renderer.UploadModel(planeModel);
        planeModel.Texture = Game.GameInfo.ResourceManager.LoadTexture("grass.png").GetAwaiter().GetResult();
        planeModel.SolidColor = Color.Green;

        Game.GameInfo.Entities.Add(new Entity
        {
            Transform = Transform.Default(30) with
            {
                Position = new Vector3(-10, 0, -10)
            },
            Model = planeModel
        });

        _surface.Configure(new GPUSurfaceConfiguration
        {
            Device = _device,
            TextureFormat = GPUTextureFormat.Bgra8UnormSrgb,
            Usage = GPUTextureUsage.RENDER_ATTACHMENT,
            Width = Game.GameInfo.ScreenWidth,
            Height = Game.GameInfo.ScreenHeight,
            PresentMode = PresentMode.Fifo
        });
    }

    static void onResize(Vector2D<int> size)
    {
        Game.GameInfo.ScreenWidth = size.X;
        Game.GameInfo.ScreenHeight = size.Y;

        _surface.Configure(new GPUSurfaceConfiguration
        {
            Device = _device,
            TextureFormat = GPUTextureFormat.Bgra8UnormSrgb,
            Usage = GPUTextureUsage.RENDER_ATTACHMENT,
            Width = size.X,
            Height = size.Y,
            PresentMode = PresentMode.Fifo
        });
    }

    static void onRender(double delta)
    {
        Game.Frame();

        _window.SwapBuffers();


    }

    public static void Main()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);

        var window = Window.Create(options);

        // while (!Debugger.IsAttached)
        // {
        //     Thread.Sleep(100);
        // }

        _window = window;

        window.Load += () => onLoad(window);
        window.FramebufferResize += onResize;
        window.Render += onRender;
        window.Run();

        // surface.Configure(new GPUSurfaceConfiguration
        // {
        //     Width = 1000,
        //     Height = 600,
        //     TextureFormat = textureFormat,
        //     Usage = GPUTextureUsage.RENDER_ATTACHMENT,
        //     Device = (GPUDevice)device,
        //     PresentMode = PresentMode.Fifo,
        //     AlphaMode = CompositeAlphaMode.Auto
        // });


        // Game.GameInfo = Game.InitializeGame(desktopImpl, device, textureFormat).GetAwaiter().GetResult();
        // SetupInput(window);

        // while (!window.Glfw.WindowShouldClose(window.GetHandle()))
        // {
        //     window.Glfw.PollEvents();
        //
        //     // UpdateMouseDeltas(window);
        //
        //     // Game.Frame();
        //
        //     // surface.Present();
        //     // window.Glfw.SwapBuffers(window.GetHandle());
        // }
    }

    private static double lastMousePosX;
    private static double lastMousePosY;

    // private static unsafe void UpdateMouseDeltas(MyWindow myWindow)
    // {
    //     myWindow.Glfw.GetCursorPos(myWindow.GetHandle(), out var xPos, out var yPos);
    //
    //     Game.GameInfo.Input.MouseChangeX = (float)(xPos - lastMousePosX);
    //     Game.GameInfo.Input.MouseChangeY = (float)(yPos - lastMousePosY);
    //
    //     lastMousePosX = xPos;
    //     lastMousePosY = yPos;
    // }
    //
    // private static unsafe void SetupInput(MyWindow myWindow)
    // {
    //     myWindow.Glfw.SetInputMode(myWindow.GetHandle(), CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);
    //
    //     if (!myWindow.Glfw.RawMouseMotionSupported())
    //     {
    //         throw new Exception("Raw Mouse Motion not supported!");
    //     }
    //
    //     myWindow.Glfw.SetInputMode(myWindow.GetHandle(), CursorStateAttribute.RawMouseMotion, true);
    //
    //     myWindow.Glfw.SetKeyCallback(myWindow.GetHandle(), KeyCallback);
    // }

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