using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class WebSocketManager
    {
        private static readonly ConcurrentBag<WebSocket> Clients = [];
        private readonly HttpListener _httpListener;

        public event Action<WebSocket, string>? MessageReceived;

        public WebSocketManager(int port)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:{port}/");
            _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");

            _httpListener.Start();
            Console.WriteLine($"WebSocket server started on ws://localhost:{port}");

            Task.Run(AcceptConnectionsAsync);
        }

        private async Task AcceptConnectionsAsync()
        {
            while (_httpListener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await _httpListener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var wsContext = await context.AcceptWebSocketAsync(null);
                        var webSocket = wsContext.WebSocket;
                        Clients.Add(webSocket);
                        Console.WriteLine("New WebSocket client connected!");

                        await SendMessageAsync(webSocket, "Welcome to the Minecraft server controller!");
                        await ReceiveMessagesAsync(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting connection: {ex.Message}");
                }
            }
        }

        private async Task ReceiveMessagesAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        Clients.TryTake(out _);
                        Console.WriteLine("WebSocket client disconnected.");
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        MessageReceived?.Invoke(webSocket, message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling message: {ex.Message}");
                    Clients.TryTake(out _);
                    break;
                }
            }
        }

        private static async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task BroadcastAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            foreach (WebSocket client in Clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting message: {ex.Message}");
                    }
                }
            }
        }

        public static void SendMessage(WebSocket webSocket, string message) =>
            Task.Run(() => SendMessageAsync(webSocket, message));
        public static void Broadcast(string message) =>
            Task.Run(() => BroadcastAsync(message));
    }
}
