using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace Game.WebGPU;

public class JsCanvas : IInteropObject
{
    public JSObject JsObject { get; init; } = null!;

    public GPUCanvasContext GetContext()
    {
        return new GPUCanvasContext
        {
            JsObject = Interop.Canvas_GetContext(JsObject, "webgpu")
        };
    }

    public void AddEventListener<T>(string type, Action<T> callback) where T : Event
    {
        Interop.Canvas_AddEventListener(JsObject, type, s =>
        {
            var @event = (T)JsonSerializer.Deserialize(s, typeof(T), InteropSerializerContext.Default)!;
            callback(@event);
        });
    }
}

