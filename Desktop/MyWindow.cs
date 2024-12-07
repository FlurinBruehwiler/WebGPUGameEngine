// using System.Runtime.InteropServices;
// using System.Runtime.Versioning;
// using Desktop.WebGPU;
// using Silk.NET.GLFW;
// using Silk.NET.Maths;
// using Silk.NET.WebGPU;
// using Silk.NET.Windowing;
//
// namespace Desktop;
//
// public class MyWindow
// {
//     public required Glfw Glfw;
//     public required IntPtr Handle;
//
//     public unsafe WindowHandle* GetHandle()
//     {
//         return (WindowHandle*)Handle;
//     }
//
//     public static unsafe IWindow? Initialize()
//     {
//         var options = WindowOptions.Default;
//         options.Size = new Vector2D<int>(800, 600);
//
//         var window = Window.Create(options);
//         window.Load += onLoad;
//         window.FramebufferResize += onResize;
//         window.Render += onRender;
//         window.Run();
//
//         return window;
//     }
//
//     static void onLoad()
//     {
//
//     }
//
//     static void onResize(Vector2D<int> size)
//     {
//
//     }
//
//     static void onRender(double delta)
//     {
//
//     }
//
//     private static void ErrorCallback(ErrorCode error, string description)
//     {
//         Console.WriteLine($"{error}: {description}");
//     }
//
//     public unsafe GPUSurface CreateSurface()
//     {
//         Surface* surface;
//
//         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//         {
//             surface = CreateWindowSurface();
//         }
//         else
//         {
//             throw new NotImplementedException("Only windows support atm");
//         }
//
//         return new GPUSurface
//         {
//             Surface = surface
//         };
//     }
//
//     [SupportedOSPlatform("windows")]
//     private unsafe Surface* CreateWindowSurface()
//     {
//         var magicStuff = new SurfaceDescriptorFromWindowsHWND
//         {
//             Chain = new ChainedStruct
//             {
//                 SType = SType.SurfaceDescriptorFromWindowsHwnd,
//                 Next = null
//             },
//             Hinstance = (void*)GetModuleHandle(null!),
//             Hwnd = (void*)Handle
//         };
//
//         var descriptor = new SurfaceDescriptor
//         {
//             NextInChain = &magicStuff.Chain,
//         };
//
//         return GPU.API.InstanceCreateSurface(GPU.Instance, in descriptor);
//     }
//
//     [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
//     public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
// }