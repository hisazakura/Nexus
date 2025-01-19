using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NexusMinecraftServer
{
    public class CommandHandler()
    {
        public static void ApplyHandler(MinecraftServer minecraftServer, FleckServer server)
        {
            server.MessageReceived += (socket, message) =>
            {
                if (!TryHandleBackendCommand(minecraftServer, server, socket, message))
                    HandleMinecraftCommand(minecraftServer, server, message);
            };
        }

        public static void Handle(MinecraftServer minecraftServer, FleckServer server, string message)
        {
            if (!TryHandleBackendCommand(minecraftServer, server, null, message))
                HandleMinecraftCommand(minecraftServer, server, message);
        }

        private static bool TryHandleBackendCommand(MinecraftServer minecraftServer, FleckServer server, IWebSocketConnection? client, string command)
        {
            switch (command)
            {
                case "start":
                    minecraftServer.Start();
                    return true;
                case "stop":
                    minecraftServer.Stop();
                    return true;
                case "servercheck":
                    string message = minecraftServer.ServerProcess != null ?
                        "[Nexus] Server is running!" : "[Nexus] Server is not running.";
                    if (client != null) server.SendMessage(message, client.ConnectionInfo.Id);
                    else Console.WriteLine(message);
                    return true;
                default:
                    return false;
            }
        }

        private static void HandleMinecraftCommand(MinecraftServer minecraftServer, FleckServer server, string command)
        {
            if (minecraftServer.ServerProcess == null)
            {
                const string message = "Unknown command or server is not running.";
                Console.WriteLine(message);
                server.Broadcast(message);
                return;
            }
            
            try
            {
                using StreamWriter writer = minecraftServer.ServerProcess.StandardInput;
                if (writer.BaseStream.CanWrite)
                {
                    writer.WriteLine(command);
                    server.Broadcast($"[Command] {command}");
                }
            }
            catch (Exception ex)
            {
                minecraftServer.Error($"[Error] Failed to send command to Minecraft server: {ex.Message}");
            }
        }
    }
}
