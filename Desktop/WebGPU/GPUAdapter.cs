using System.Runtime.InteropServices;
using Client.WebGPU;
using Silk.NET.WebGPU;

namespace Desktop.WebGPU;

public unsafe class GPUAdapter : IGPUAdapter
{
    public required Adapter* Adapter;

    public Task<IGPUDevice> RequestDevice()
    {
        var deviceDescriptor = new DeviceDescriptor();

        var taskCompletionSource = new TaskCompletionSource<IGPUDevice>();

        var handle = GCHandle.Alloc(taskCompletionSource);

        GPU.API.AdapterRequestDevice(Adapter, in deviceDescriptor, new PfnRequestDeviceCallback(OnDeviceRequestEnded), ref handle);

        return taskCompletionSource.Task;
    }

    private static void OnDeviceRequestEnded(RequestDeviceStatus status, Device* device, byte* data, void* userData)
    {
        GCHandle handle = GCHandle.FromIntPtr(new IntPtr(userData));
        var tcs = (TaskCompletionSource<IGPUDevice>)handle.Target!;
        switch (status)
        {
            case RequestDeviceStatus.Success:
                tcs.SetResult(new GPUDevice
                {
                    Device = device
                });
                break;
            default:
                tcs.SetException(new Exception(status.ToString()));
                break;
        }
    }
}