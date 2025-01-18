using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NexusMinecraftServer
{
    public class CommandHandler()
    {
        public static void Handle(MinecraftServer minecraftServer, FleckServerCollection collection, FleckServer? server, string command)
        {
            switch (command)
            {
                case "start":
                    minecraftServer.Start();
                    break;
                case "stop":
                    minecraftServer.Stop();
                    break;
                case "servercheck":
                    string message = minecraftServer.ServerProcess != null ?
                        "[Nexus] Server is running!" : "[Nexus] Server is not running.";
                    if (server != null) server.SendMessage(message);
                    else Console.WriteLine(message);
                    break;
                default:
                    HandleCustom(minecraftServer, collection, server, command);
                    break;
            }
        }

        private static void HandleCustom(MinecraftServer minecraftServer, FleckServerCollection collection, FleckServer? _, string command)
        {
            if (minecraftServer.ServerProcess != null)
            {
                try
                {
                    using StreamWriter writer = minecraftServer.ServerProcess.StandardInput;
                    if (writer.BaseStream.CanWrite)
                    {
                        writer.WriteLine(command);
                        collection.Broadcast($"[Command] {command}");
                    }
                }
                catch (Exception ex)
                {
                    minecraftServer.Error($"[Error] Failed to send command to Minecraft server: {ex.Message}");
                }
            }
            else
            {
                const string message = "Unknown command or server is not running.";
                Console.WriteLine(message);
                collection.Broadcast(message);
            }
        }
    }
}
