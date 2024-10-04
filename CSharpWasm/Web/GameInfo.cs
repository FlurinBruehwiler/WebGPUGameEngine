using System.Collections.Generic;
using System.Numerics;
using WasmTestCSharp.WebGPU;

namespace WasmTestCSharp;

public class GameInfo
{
    public List<Vector3> Vertices = [];
    public required GPURenderPipeline RenderPipeline;
    public required GPUDevice Device;
    public required GPUCanvasContext Context;
    public int ScreenWidth;
    public int ScreenHeight;
    public required Canvas Canvas;

    public void UpdateScreenDimensions()
    {
        ScreenWidth = Canvas.JsObject.GetPropertyAsInt32("width");
        ScreenHeight = Canvas.JsObject.GetPropertyAsInt32("height");
    }
}
