using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace WasmTestCSharp.WebGPU;

public class GPUAdapter : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public async Task<GPUDevice> RequestDevice()
    {
        return new GPUDevice
        {
            JsObject = await Interop.GPUAdapter_requestDevice(JsObject)
        };
    }
}
