using System.Net.WebSockets;
using Shared;

namespace GameEngine;

public class Server
{
    private ClientWebSocket WebSocket = new();
    public Queue<IMessage> PendingReceivingMessages = [];
    public Queue<IMessage> PendingSendingMessages = [];

    public async Task<bool> TryEstablishConnection()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        await WebSocket.ConnectAsync(new Uri("ws://localhost:7235/ws"), cts.Token);

        if (cts.IsCancellationRequested)
        {
            Console.WriteLine("Couldn't connect to server :(");
        }

        return true;
    }

    public ValueTask SendMessage(IMessage message)
    {
        return Networking.SendMessage(WebSocket, message);
    }

    public ValueTask<IMessage?> GetNextMessage()
    {
        return Networking.GetNextMessage(WebSocket);
    }

    public async ValueTask ListenForMessages()
    {
        while (true)
        {
            var message = await GetNextMessage();

            if (message == null)
            {
                await Task.Delay(1000);
                return;
            }

            PendingReceivingMessages.Enqueue(message);
        }
    }
}