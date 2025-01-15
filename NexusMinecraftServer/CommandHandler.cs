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
        public static void Handle(MinecraftServer minecraftServer, WebSocket client, string command)
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
                    WebSocketManager.SendMessage(client, minecraftServer.ServerProcess != null ? 
                        "[Nexus] Server is running!" : "[Nexus] Server is not running.");
                    break;
                default:
                    HandleCustom(minecraftServer, client, command);
                    break;
            }
        }

        private static void HandleCustom(MinecraftServer minecraftServer, WebSocket client, string command)
        {
            if (minecraftServer.ServerProcess != null)
            {
                try
                {
                    using StreamWriter writer = minecraftServer.ServerProcess.StandardInput;
                    if (writer.BaseStream.CanWrite)
                    {
                        writer.WriteLine(command);
                        WebSocketManager.Broadcast($"[Command] {command}");
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
                WebSocketManager.Broadcast(message);
            }
        }
    }
}
