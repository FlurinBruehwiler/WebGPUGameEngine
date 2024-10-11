using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public static unsafe class GPU
{
    private static NativeGPU? _api;
    public static NativeGPU API
    {
        get
        {
            return _api ??= NativeGPU.GetApi();
        }
    }

    private static Instance* _instance;
    public static Instance* Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            InstanceDescriptor instanceDescriptor = new InstanceDescriptor();
            _instance = API.CreateInstance(in instanceDescriptor);
            return _instance;
        }
    }

    public static Task<GPUAdapter> RequestAdapter(GPURequestAdapterOptions options)
    {
        var taskCompletionSource = new TaskCompletionSource<GPUAdapter>();

        var handle = GCHandle.Alloc(taskCompletionSource);

        var nativeOptions = new RequestAdapterOptions
        {
            CompatibleSurface = options.CompatibleSurface.Surface
        };

        API.InstanceRequestAdapter(Instance, nativeOptions, new PfnRequestAdapterCallback(OnAdapterRequestEnded), ref handle);

        return taskCompletionSource.Task;
    }

    private static void OnAdapterRequestEnded(RequestAdapterStatus status, Adapter* adapter, byte* data, void* userData)
    {
        GCHandle handle = GCHandle.FromIntPtr(new IntPtr(userData));
        var tcs = (TaskCompletionSource<GPUAdapter>)handle.Target!;
        switch (status)
        {
            case RequestAdapterStatus.Success:
                tcs.SetResult(new GPUAdapter
                {
                    Adapter = adapter
                });
                break;
            default:
                tcs.SetException(new Exception(status.ToString()));
                break;
        }
    }
}

public struct GPURequestAdapterOptions
{
    public GPUSurface CompatibleSurface;
}