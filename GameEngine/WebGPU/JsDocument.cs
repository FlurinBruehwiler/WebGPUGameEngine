using System.Text.Json;

namespace GameEngine.WebGPU;

public static class JsDocument
{
    public static T QuerySelector<T>(string selector) where T : IInteropObject, new()
    {
        return new T
        {
            JsObject = Interop.Document_QuerySelector(selector)
        };
    }

    public static void AddEventListener<T>(string type, Action<T> callback) where T : Event
    {
        Interop.Document_AddEventListener(type, s =>
        {
            var @event = (T)JsonSerializer.Deserialize(s, typeof(T), InteropSerializerContext.Default)!;
            callback(@event);
        });
    }
}

public class Event
{

}

public class MouseEvent : Event
{
    public int MovementX { get; set; }
    public int MovementY { get; set; }
}

public class KeyboardEvent : Event
{
    public string Code { get; set; }
}