using System.Runtime.InteropServices.JavaScript;

namespace Web.WebGPU;

public interface IInteropObject
{
    JSObject JsObject { get; init; }
}
