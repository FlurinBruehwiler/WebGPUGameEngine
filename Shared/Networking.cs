using System.Diagnostics;
using System.Net.WebSockets;
using MemoryPack;

namespace Shared;

public static class Networking
{
    public static unsafe ValueTask SendMessage(WebSocket webSocket, IMessage message)
    {
        if (webSocket.State == WebSocketState.Closed) //not sure exactly where to place this???
            return ValueTask.CompletedTask;

        var binaryMessage = MemoryPackSerializer.Serialize(message);

        var length = binaryMessage.Length;

        var messageBuffer = new byte[length + 4];
        new Span<byte>(&length, 4).CopyTo(messageBuffer);
        binaryMessage.CopyTo(messageBuffer, 4);

        return webSocket.SendAsync(messageBuffer, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }

    public static async ValueTask<IMessage?> GetNextMessage(WebSocket webSocket)
    {
        if (webSocket.State == WebSocketState.Closed) //not sure exactly where to place this???
            return null;

        byte[] header = new byte[4];
        await ReadExactlyAsync(webSocket, header);
        var length = ConvertToInt(header);

        byte[] messageAsBytes = new byte[length];
        await ReadExactlyAsync(webSocket, messageAsBytes);

        return MemoryPackSerializer.Deserialize<IMessage>(messageAsBytes);
    }

    private static async ValueTask ReadExactlyAsync(WebSocket webSocket, Memory<byte> buffer) //very simplistic WS implementation, needs to be reworked
    {
        Console.WriteLine(webSocket.State);

        var currentPosition = 0;

        while (true)
        {
            var res = await webSocket.ReceiveAsync(buffer.Slice(currentPosition), CancellationToken.None);

            currentPosition += res.Count;

            if (currentPosition == buffer.Length)
            {
                Debug.Assert(res.EndOfMessage);
                return;
            }
        }
    }

    private static unsafe int ConvertToInt(byte[] array)
    {
        fixed (byte* firstChar = array)
        {
            int* i = (int*)firstChar;
            return *i;
        }
    }
}