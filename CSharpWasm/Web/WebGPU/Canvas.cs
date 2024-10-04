using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

public class Canvas : IInteropObject
{
    public required JSObject JsObject { get; init; }

    public static Canvas Search(string selector)
    {
        return new Canvas
        {
            JsObject = Interop.Document_QuerySelector(selector)
        };
    }

    public GPUCanvasContext GetContext()
    {
        return new GPUCanvasContext
        {
            JsObject = Interop.Canvas_GetContext(JsObject, "webgpu")
        };
    }
}

