using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using GameEngine.WebGPU;

namespace WasmTestCSharp.WebGPU;

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
