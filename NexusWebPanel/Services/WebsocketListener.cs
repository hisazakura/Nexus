using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace NexusWebPanel.Services
{
    public class WebsocketListener(int port)
    {
        private ClientWebSocket _webSocket = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        private Task? _monitorPortTask;
        private Task? _listeningTask;

        public int Port { get; } = port;
        public bool IsConnected => _webSocket != null &&
            _webSocket.State == WebSocketState.Open;

        public event Action? Connected;
        public event Action? Disconnected;
        public event Action<string>? MessageReceived;

        public void Start()
        {
            if (_monitorPortTask != null || _listeningTask != null)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _monitorPortTask = Task.Run(MonitorPortAvailability);
                _listeningTask = Task.Run(StartListening);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting WebSocketController: {ex.Message}");
                Stop();
                Disconnected?.Invoke();
            }
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();

                _monitorPortTask?.Wait();
                _listeningTask?.Wait();

                if (_webSocket.State == WebSocketState.Open ||
                    _webSocket.State == WebSocketState.Connecting)
                    _webSocket.Abort();

                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();

                Disconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error stopping WebSocketController: {ex.Message}");
            }
            finally
            {
                _monitorPortTask = null;
                _listeningTask = null;
            }
        }

        private async Task MonitorPortAvailability()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected) await ConnectAsync();
                }
                catch (Exception) { }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                _webSocket?.Dispose();
                _webSocket = new ClientWebSocket();

                Uri serverUri = new($"ws://127.0.0.1:{Port}");
                await _webSocket.ConnectAsync(serverUri, _cancellationTokenSource.Token);
                Connected?.Invoke();
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
                    MessageReceived?.Invoke(message);
                }
                catch (WebSocketException wsEx)
                {
                    Trace.TraceError($"WebSocket exception: {wsEx.Message}");
                    Disconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error receiving WebSocket message: {ex.Message}");
                    Disconnected?.Invoke();
                }
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
                Disconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error sending WebSocket message: {ex.Message}");
                Disconnected?.Invoke();
            }
        }

        public void SendMessage(string message)
        {
            Task.Run(() => SendMessageAsync(message, _cancellationTokenSource.Token));
        }
    }
}
