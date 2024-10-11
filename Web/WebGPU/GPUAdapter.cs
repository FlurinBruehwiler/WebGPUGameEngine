using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Client.WebGPU;

namespace Web.WebGPU;

public class GPUAdapter : IInteropObject, IGPUAdapter
{
    public required JSObject JsObject { get; init; }

    public async Task<IGPUDevice> RequestDevice()
    {
        return new GPUDevice
        {
            JsObject = await Interop.GPUAdapter_RequestDevice(JsObject)
        };
    }
}
