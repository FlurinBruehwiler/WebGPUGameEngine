using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Desktop.WebGPU;
using Silk.NET.GLFW;
using Silk.NET.WebGPU;

namespace Desktop;

public class Window
{
    public required Glfw Glfw;
    public required IntPtr Handle;

    public unsafe WindowHandle* GetHandle()
    {
        return (WindowHandle*)Handle;
    }

    public static unsafe Window? Initialize()
    {
        var api = Glfw.GetApi();

        api.SetErrorCallback(ErrorCallback);

        if (!api.Init())
        {
            Console.WriteLine("Init failed");
            return null;
        }

        api.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        api.WindowHint(WindowHintBool.Resizable, false);

        var window = api.CreateWindow(1000, 600, "WebGPU", null, null);
        if (window == null)
        {
            Console.WriteLine("Window creation failed");
            return null;
        }

        return new Window
        {
            Glfw = api,
            Handle = (IntPtr)window
        };
    }

    // private static void ErrorCallback(ErrorCode error, string description)
    // {
    //     Console.WriteLine($"{error}: {description}");
    // }

    // public unsafe GPUSurface CreateSurface()
    // {
    //     Surface* surface;
    //
    //     if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //     {
    //         surface = CreateWindowSurface();
    //     }
    //     else
    //     {
    //         throw new NotImplementedException("Only windows support atm");
    //     }
    //
    //     return new GPUSurface
    //     {
    //         Surface = surface
    //     };
    // }

    // [SupportedOSPlatform("windows")]
    // private unsafe Surface* CreateWindowSurface()
    // {
    //     var magicStuff = new SurfaceDescriptorFromWindowsHWND
    //     {
    //         Chain = new ChainedStruct
    //         {
    //             SType = SType.SurfaceDescriptorFromWindowsHwnd,
    //             Next = null
    //         },
    //         Hinstance = (void*)GetModuleHandle(null!),
    //         Hwnd = (void*)Handle
    //     };
    //
    //     var descriptor = new SurfaceDescriptor
    //     {
    //         NextInChain = &magicStuff.Chain,
    //     };
    //
    //     return GPU.API.InstanceCreateSurface(GPU.Instance, in descriptor);
    // }

    [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
    public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
}