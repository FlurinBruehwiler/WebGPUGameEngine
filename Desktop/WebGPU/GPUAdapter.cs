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

        GPU.API.AdapterRequestDevice(Adapter, in deviceDescriptor, PfnRequestDeviceCallback.From((status, device, _, _) =>
        {
            switch (status)
            {
                case RequestDeviceStatus.Success:
                    taskCompletionSource.SetResult(new GPUDevice
                    {
                        Device = device
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