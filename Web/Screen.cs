using GameEngine;
using WasmTestCSharp.WebGPU;

namespace WasmTestCSharp;

public class Screen : IScreen
{
    public required JsCanvas Canvas;

    public int GetWidth()
    {
        return Canvas.JsObject.GetPropertyAsInt32("width");
    }

    public int GetHeight()
    {
        return Canvas.JsObject.GetPropertyAsInt32("height");
    }
}