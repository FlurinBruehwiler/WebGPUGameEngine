using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

/// <summary>
/// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue
/// </summary>
public class GPUQueue : IInteropObject
{
    public required JSObject JsObject { get; init; }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/writeBuffer
    /// </summary>
    public void WriteBuffer(GPUBuffer buffer, int bufferOffset, float[] data, int dataOffset, int size)
    {
        Interop.GPUQueue_WriteBuffer(JsObject, buffer.JsObject, bufferOffset, data, dataOffset, size);
    }

    /// <summary>
    /// https://developer.mozilla.org/en-US/docs/Web/API/GPUQueue/submit
    /// </summary>
    public void Submit(GPUCommandBuffer[] commandBuffers)
    {
        Interop.GPUQueue_Submit(JsObject, commandBuffers.Select(x => x.JsObject).ToArray());
    }
}
