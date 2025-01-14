using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nexus.Services.Minecraft
{
    public class MinecraftWebsocketController(int port = 8080)
    {
        private ClientWebSocket _webSocket = new();
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _monitorPortTask;
        private Task? _serverChekerTask;
        private Task? _listeningTask;
        private readonly int _port = port;
        private readonly string _serverCheckMessage = "servercheck";

        public event Action<string>? OnMessageReceived;
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public bool IsConnected => _webSocket != null &&
            _webSocket.State == WebSocketState.Open;

        public void Start()
        {
            if (_monitorPortTask != null || _listeningTask != null || _serverChekerTask != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _monitorPortTask = Task.Run(MonitorPortAvailability);
                _listeningTask = Task.Run(StartListening);
                _serverChekerTask = Task.Run(StartServerCheck);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting WebSocketController: {ex.Message}");
                Stop();
                OnDisconnected?.Invoke();
            }
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();

                _monitorPortTask?.Wait();
                _listeningTask?.Wait();
                _serverChekerTask?.Wait();

                if (_webSocket.State == WebSocketState.Open ||
                    _webSocket.State == WebSocketState.Connecting)
                    _webSocket.Abort();

                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();

                OnDisconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error stopping WebSocketController: {ex.Message}");
            }
            finally
            {
                _monitorPortTask = null;
                _listeningTask = null;
                _serverChekerTask = null;
            }
        }

        private async Task MonitorPortAvailability()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected) await ConnectAsync(_cancellationTokenSource.Token);
                }
                catch (Exception) { }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }

        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                _webSocket?.Dispose();
                _webSocket = new ClientWebSocket();

                Uri serverUri = new($"ws://127.0.0.1:{_port}");
                await _webSocket.ConnectAsync(serverUri, cancellationToken);
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error connecting to WebSocket: {ex.Message}");
                throw;
            }
        }

        private async Task StartListening()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected) continue;

                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult result =
                        await _webSocket.ReceiveAsync(buffer, _cancellationTokenSource.Token);

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(message);
                }
                catch (WebSocketException wsEx)
                {
                    Trace.TraceError($"WebSocket exception: {wsEx.Message}");
                    OnDisconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error receiving WebSocket message: {ex.Message}");
                    OnDisconnected?.Invoke();
                }
            }

        }

        private async Task StartServerCheck()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected) continue;

                    await SendMessageAsync(_serverCheckMessage, _cancellationTokenSource.Token);

                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error sending server check: {ex.Message}");
                    OnDisconnected?.Invoke();
                }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }

        private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            if (!IsConnected) return;

            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, cancellationToken);
            }
            catch (WebSocketException wsEx)
            {
                Trace.TraceError($"WebSocket exception during send: {wsEx.Message}");
                OnDisconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error sending WebSocket message: {ex.Message}");
                OnDisconnected?.Invoke();
            }
        }

        public void SendMessage(string message)
        {
            Task.Run(() => SendMessageAsync(message, _cancellationTokenSource.Token));
        }
    }
}
