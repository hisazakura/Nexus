using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.WebPanel
{
    public class WebPanelServerController(int port)
    {
        private HttpClient _httpClient = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        private Task? _monitorTask;

        private readonly int _port = port;

        public event Action? Connected;
        public event Action? Disconnected;

        public void Start()
        {
            if (_monitorTask != null) return;
            _monitorTask = Task.Run(MonitorPort);
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                _monitorTask?.Wait();
                Disconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error stopping WebSocketController: {ex.Message}");
            }
            finally
            {
                _monitorTask = null;
            }
        }

        private async Task MonitorPort()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (await TryConnect()) Connected?.Invoke();
                    else Disconnected?.Invoke();
                }
                catch (Exception) { }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }

        private async Task<bool> TryConnect()
        {
            Uri tunnelsEndpoint = new($"http://127.0.0.1:{_port}/api/status");
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(tunnelsEndpoint);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Trace.TraceError($"Request error: {e.Message}");
            }

            return false;
        }
    }
}
