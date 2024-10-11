using Shared;

namespace Server;

public class Program
{
    public static List<Client> Clients = [];
    public static List<NetworkEntity> Entities = [];

    public static void Main()
    {
        var builder = WebApplication.CreateSlimBuilder();
        var app = builder.Build();

        app.UseWebSockets();

        app.MapGet("/", () => "Server alive!");

        app.Map("/ws", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 404;
                return;
            }

            using var ws = await context.WebSockets.AcceptWebSocketAsync();

            Console.WriteLine("Client connected");

            var client = new Client
            {
                WebSocket = ws
            };
            Clients.Add(client);

            await SendAllEntitiesToClient(client);

            _ = client.ListenForMessages().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Console.WriteLine(t.Exception);
                }

                Console.WriteLine("Client disconnected");
            });
        });

        app.Run();
    }

    public static async ValueTask SendAllEntitiesToClient(Client client)
    {
        foreach (var networkEntity in Entities)
        {
            await client.SendMessage(new CreateMessage
            {
                ModelId = networkEntity.ModelId,
                EntityId = networkEntity.Id,
                Transform = networkEntity.Transform
            });
        }
    }
}