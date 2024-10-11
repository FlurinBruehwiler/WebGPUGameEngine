using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using GameEngine.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe static class GPU
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

    public static unsafe Task<IGPUAdapter> RequestAdapter()
    {
        var taskCompletionSource = new TaskCompletionSource<IGPUAdapter>();

        var handle = GCHandle.Alloc(taskCompletionSource);

        API.InstanceRequestAdapter(Instance, null, new PfnRequestAdapterCallback(OnAdapterRequestEnded), ref handle);

        return taskCompletionSource.Task;
    }

    private static void OnAdapterRequestEnded(RequestAdapterStatus status, Adapter* adapter, byte* data, void* userData)
    {
        GCHandle handle = GCHandle.FromIntPtr(new IntPtr(userData));
        var tcs = (TaskCompletionSource<IGPUAdapter>)handle.Target!;
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