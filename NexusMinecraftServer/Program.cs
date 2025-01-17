using NexusMinecraftServer;

bool _keepRunning = true;

ServerConfig config = new(args);
config.Validate();

// Open two ports because one port can only handle one connection at a time
MinecraftServer minecraftServer = new(config);
WebSocketManager backendWebSocketManager = new(8080);
WebSocketManager publicWebSocketManager = new(8081);

backendWebSocketManager.MessageReceived += (client, message) =>
    CommandHandler.Handle(minecraftServer, client, message);
publicWebSocketManager.MessageReceived += (client, message) =>
    CommandHandler.Handle(minecraftServer, client, message);

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Shutting down...");
    _keepRunning = false;
    e.Cancel = true;
};

Console.WriteLine("Server console ready. Type commands or press Ctrl+C to exit.");

Task inputTask = Task.Run(() =>
{
    while (_keepRunning)
    {
        string? input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input))
            CommandHandler.Handle(minecraftServer, null!, input);
    }
});

while (_keepRunning)
    await Task.Delay(1000);

await inputTask;

if (minecraftServer.ServerProcess != null)
    minecraftServer.Stop();

Console.WriteLine("Application exited.");