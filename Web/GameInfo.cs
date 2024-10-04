using System;
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
    public required JsCanvas JsCanvas;
    public Camera Camera;
    public Input Input = new();

    public void UpdateScreenDimensions()
    {
        ScreenWidth = JsCanvas.JsObject.GetPropertyAsInt32("width");
        ScreenHeight = JsCanvas.JsObject.GetPropertyAsInt32("height");
    }
}

public class Input
{
    private HashSet<string> pressedKeys = [];

    public float MouseChangeX;
    public float MouseChangeY;

    public void InformKeyChanged(string code, bool down)
    {
        if (down)
        {
            pressedKeys.Add(code);
        }
        else
        {
            pressedKeys.Remove(code);
        }
    }

    public bool IsKeyDown(string code)
    {
        return pressedKeys.Contains(code);
    }

    public void NextFrame()
    {
        MouseChangeX = 0;
        MouseChangeY = 0;
    }
}
