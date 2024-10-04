using System.Runtime.InteropServices.JavaScript;

namespace WasmTestCSharp.WebGPU;

public interface IInteropObject
{
    JSObject JsObject { get; init; }
}
