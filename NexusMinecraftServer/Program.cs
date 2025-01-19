using NexusMinecraftServer;

bool _keepRunning = true;

ServerConfig config = new(args);
config.Validate();

MinecraftServer minecraftServer = new(config);
FleckServer server = new(8080);
server.Start();

minecraftServer.LogReceived += message => server.Broadcast(message);
minecraftServer.ErrorReceived += message => server.Broadcast(message);

CommandHandler.ApplyHandler(minecraftServer, server);

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Shutting down...");
    _keepRunning = false;
    e.Cancel = true;
    server.Stop();
};

Console.WriteLine("Server console ready. Type commands or press Ctrl+C to exit.");

Task inputTask = Task.Run(() =>
{
    while (_keepRunning)
    {
        string? input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input))
            CommandHandler.Handle(minecraftServer, server, input);
    }
});

while (_keepRunning)
    await Task.Delay(1000);

await inputTask;

if (minecraftServer.ServerProcess != null)
    minecraftServer.Stop();

Console.WriteLine("Application exited.");