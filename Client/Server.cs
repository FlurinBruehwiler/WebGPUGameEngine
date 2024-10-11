using System.Net.WebSockets;
using System.Numerics;
using Shared;

namespace Client;

public class Server
{
    private ClientWebSocket WebSocket = new();
    public Queue<IMessage> PendingReceivingMessages = [];
    public Queue<IMessage> PendingSendingMessages = [];

    public async Task<bool> TryEstablishConnection()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        try
        {
            await WebSocket.ConnectAsync(new Uri("ws://localhost:5105/ws"), cts.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unable to connect to server because: {e.Message}");
            return false;
        }

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

    private ValueTask<IMessage?> GetNextMessage()
    {
        return Networking.GetNextMessage(WebSocket);
    }

    public async Task SendPendingMessages()
    {
        while (PendingSendingMessages.TryDequeue(out var message))
        {
            await SendMessage(message);
        }
    }

    public async ValueTask ListenForMessages()
    {
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void ProcessServerMessages()
    {
        while (PendingReceivingMessages.TryDequeue(out var message))
        {
            if (message is UpdateMessage updateMessage)
            {
                Console.WriteLine("Got update message");
                var entity = Game.GameInfo.Entities.First(x => x.Id == updateMessage.EntityId);
                entity.Transform = Transform.FromNetwork(updateMessage.Transform);
            }
            else if (message is CreateMessage createMessage)
            {
                Console.WriteLine("Got create message");
                Game.GameInfo.Entities.Add(new Entity
                {
                    Id = createMessage.EntityId,
                    Transform = Transform.FromNetwork(createMessage.Transform),
                    Model = Game.GameInfo.ResourceManager.GetModelFromId(createMessage.ModelId)
                });
            }
        }
    }

    private static void Frame()
    {
        Game.GameInfo.Server?.ProcessServerMessages();

        Game.UpdateCamera();

        SendUpdatesToServer();
        _ = Game.GameInfo.Server?.SendPendingMessages(); //async

        Renderer.RenderFrame(Game.GameInfo.Camera);

        Game.GameInfo.Input.NextFrame();

        //request next frame
        JsWindow.RequestAnimationFrame(FrameCatch);
    }

    private static Guid PlayerGuid = Guid.NewGuid();

    private static void FrameCatch()
    {
        try
        {
            Frame();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void SendUpdatesToServer()
    {
        if (Game.GameInfo.Server == null)
            return;

        var server = Game.GameInfo.Server;

        server.PendingSendingMessages.Enqueue(new UpdateMessage
        {
            EntityId = PlayerGuid,
            Transform = Game.GameInfo.Camera.Transform.ToNetwork()
        });
    }

}