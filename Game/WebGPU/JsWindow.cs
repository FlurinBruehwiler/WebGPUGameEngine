namespace Game.WebGPU;

public static class JsWindow
{
    public static void RequestAnimationFrame(Action callback)
    {
        Interop.Window_RequestAnimationFrame(callback);
    }
}