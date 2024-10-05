using System.Runtime.InteropServices.JavaScript;

namespace GameEngine.WebGPU;

public interface IInteropObject
{
    JSObject JsObject { get; init; }
}
