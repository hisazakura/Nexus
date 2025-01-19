using NexusMinecraftServer;

CancellationTokenSource cancellationTokenSource = new();

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
    cancellationTokenSource.Cancel();
    e.Cancel = true;
    server.Stop();
};

Console.WriteLine("Server console ready. Type commands or press Ctrl+C to exit.");

Task inputTask = Task.Run(() =>
{
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        try
        {
            string? input = ConsoleHelper.ReadLine(cancellationTokenSource.Token);
            if (!string.IsNullOrEmpty(input))
                CommandHandler.Handle(minecraftServer, server, input);
        }
        catch (OperationCanceledException)
        {
            // Task was canceled; exit gracefully
        }
    }
});

while (!cancellationTokenSource.Token.IsCancellationRequested)
    await Task.Delay(1000);

await inputTask;

if (minecraftServer.ServerProcess != null)
    minecraftServer.Stop();

Console.WriteLine("Application exited.");