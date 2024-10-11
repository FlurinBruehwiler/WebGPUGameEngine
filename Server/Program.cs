namespace Server;

public class Program
{
    public static List<Client> Clients = [];
    public static List<NetworkEntity> Entities = [];

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        app.UseWebSockets();

        app.MapGet("/", () => "Server alive!");

        app.Map("/wd", async context =>
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

            _ = client.ListenForMessages().ContinueWith(t => Console.WriteLine("Client disconnected"));
        });

        app.Run();
    }
}