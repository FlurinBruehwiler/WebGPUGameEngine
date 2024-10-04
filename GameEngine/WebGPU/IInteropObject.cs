using System.Runtime.InteropServices.JavaScript;

namespace Game.WebGPU;

public interface IInteropObject
{
    JSObject JsObject { get; init; }
}
