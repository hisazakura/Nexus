using NexusMinecraftServer;

bool _keepRunning = true;

ServerConfig config = new(args);
config.Validate();

// Open two ports because one port can only handle one connection at a time
MinecraftServer minecraftServer = new(config);
FleckServerCollection fleckServerCollection = new();
FleckServer backendfleckServer = fleckServerCollection.Create(8080);
FleckServer publicfleckServer = fleckServerCollection.Create(8081);

minecraftServer.LogReceived += (message) =>
    fleckServerCollection.Broadcast(message);
minecraftServer.ErrorReceived += (message) =>
    fleckServerCollection.Broadcast(message);

fleckServerCollection.MessageReceived += (client, message) =>
    CommandHandler.Handle(minecraftServer, fleckServerCollection, client, message);

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Shutting down...");
    _keepRunning = false;
    e.Cancel = true;
    fleckServerCollection.Stop();
};

Console.WriteLine("Server console ready. Type commands or press Ctrl+C to exit.");

Task inputTask = Task.Run(() =>
{
    while (_keepRunning)
    {
        string? input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input))
            CommandHandler.Handle(minecraftServer, fleckServerCollection, null!, input);
    }
});

while (_keepRunning)
    await Task.Delay(1000);

await inputTask;

if (minecraftServer.ServerProcess != null)
    minecraftServer.Stop();

Console.WriteLine("Application exited.");