using System.Net.WebSockets;
using System.Text.Json;
using Shared;

namespace Server;

public class Client
{
    public required WebSocket WebSocket;

    public ValueTask SendMessage(IMessage message)
    {
        return Networking.SendMessage(WebSocket, message);
    }

    public ValueTask<IMessage?> GetNextMessage()
    {
        return Networking.GetNextMessage(WebSocket);
    }

    public async Task ListenForMessages()
    {
        try
        {
            while (true)
            {
                var message = await GetNextMessage();

                if (message == null)
                {
                    await Task.Delay(1000);
                    continue;
                }

                Console.WriteLine($"Got Message {JsonSerializer.Serialize(message)}");

                if (message is UpdateMessage updateMessage)
                {
                    Program.Entities.First(x => x.Id == updateMessage.EntityId).Transform = updateMessage.Transform;
                    await DistributeMessageToOtherClients(updateMessage);
                }
                else if (message is CreateMessage createMessage)
                {
                    Program.Entities.Add(new NetworkEntity
                    {
                        Transform = createMessage.Transform,
                        Id = createMessage.EntityId
                    });
                    await DistributeMessageToOtherClients(createMessage);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task DistributeMessageToOtherClients(IMessage message)
    {
        foreach (var client in Program.Clients)
        {
            if(client == this)
                continue;

            await client.SendMessage(message);
        }
    }
}