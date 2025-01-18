using Microsoft.VisualBasic;
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
    // broo you stupid cunt make this shit properly you fucktard
    public class WebSocketServer
    {
        private HttpListener _httpListener;
        private WebSocket? _webSocket;

        private Task? _serverTask;
        private Task? _websocketListener;
        private Task? _websocketSender;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken => _cancellationTokenSource.Token;

        public int Port { get; }
        public bool IsClientConnected => _webSocket != null && _webSocket.State == WebSocketState.Open;

        public event Action<string>? MessageReceived;
        public event Action? ClientConnected;
        public event Action? ClientDisconnected;

        public WebSocketServer(int port)
        {
            Port = port;

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:{port}/");
            _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");

            _cancellationTokenSource = new();
            _serverTask = Task.Run(StartAsync);
        }

        public async Task StartAsync()
        {
            _httpListener.Start();
            Console.WriteLine("WebSocket server started and listening for connections...");

            while (!_cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext context = await _httpListener.GetContextAsync();

                // Reject non-websocket requests
                if (!context.Request.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    continue;
                }

                // Reject multiple clients
                if (IsClientConnected)
                {
                    context.Response.StatusCode = 403;
                    context.Response.Close();
                    Console.WriteLine("Connection rejected: Only one client is allowed.");
                    continue;
                }

                HttpListenerWebSocketContext wsContext =
                    await context.AcceptWebSocketAsync(null);
                _webSocket = wsContext.WebSocket;

                ClientConnected?.Invoke();
                Console.WriteLine("Client connected.");

                if (_webSocket != null)
                    _websocketListener = Task.Run(() => ReceiveMessages(_webSocket));
            }

            _httpListener.Stop();
        }

        private async Task ReceiveMessages(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open && !_cancellationToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult result =
                        await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", _cancellationToken);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                ClientDisconnected?.Invoke();
                Console.WriteLine("Client disconnected.");
            }
        }

        public void SendMessage(string message)
        {
            if (_webSocket != null)
                _websocketSender = Task.Run(() => SendMessageAsync(_webSocket, message));
        }

        private static async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
            {
                Console.WriteLine("Cannot send message: No client connected.");
                return;
            }

            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            if (_webSocket != null)
            {
                if (_webSocket.State == WebSocketState.Open)
                    _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None).Wait();
                _webSocket.Dispose();
                _webSocket = null;
            }

            _websocketListener?.Wait();
            _websocketSender?.Wait();

            _httpListener.Stop();
            _httpListener.Close();

            Console.WriteLine("WebSocket server stopped.");
        }
    }
}
