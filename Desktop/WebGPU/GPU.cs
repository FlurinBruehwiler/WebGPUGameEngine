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
            //new NativeGPU(NativeGPU.CreateDefaultContext(["dawn.dll"]));
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

        var nativeOptions = new RequestAdapterOptions
        {
            CompatibleSurface = options.CompatibleSurface.Surface,
            BackendType = BackendType.D3D12
        };

        API.InstanceRequestAdapter(Instance, in nativeOptions, PfnRequestAdapterCallback.From(
            (status, adapter, _, _) =>
            {
                switch (status)
                {
                    case RequestAdapterStatus.Success:
                        taskCompletionSource.SetResult(new GPUAdapter
                        {
                            Adapter = adapter
                        });
                        break;
                    default:
                        taskCompletionSource.SetException(new Exception(status.ToString()));
                        break;
                }
            }), null);

        return taskCompletionSource.Task;
    }
}

public struct GPURequestAdapterOptions
{
    public GPUSurface CompatibleSurface;
}