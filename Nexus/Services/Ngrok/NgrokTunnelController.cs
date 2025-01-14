using Newtonsoft.Json;
using Nexus.Data.Ngrok;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nexus.Services.Ngrok
{
    public class NgrokTunnelController(int port)
    {
        private HttpClient _httpClient = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        private Task? _monitorTask;

        private readonly int _port = port;

        public event Action<Dictionary<string, string>>? EndpointsReceived;
        public event Action? Disconnected;

        public void Start()
        {
            if (_monitorTask != null) return;

            _monitorTask = Task.Run(MonitorNgrokEndpoints);
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

        private async Task MonitorNgrokEndpoints()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    Dictionary<string, string>? endpoints = await FetchEndpoints();
                    if (endpoints != null) EndpointsReceived?.Invoke(endpoints);
                }
                catch (Exception) { }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }

        private async Task<Dictionary<string, string>?> FetchEndpoints()
        {
            Uri tunnelsEndpoint = new($"http://127.0.0.1:{_port}/api/tunnels");
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(tunnelsEndpoint);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return GetTunnelNamesAndUrls(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                Disconnected?.Invoke();
            }

            return null;
        }

        private static Dictionary<string, string> GetTunnelNamesAndUrls(string json)
        {
            var dictionary = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(json)) return dictionary;

            try
            {
                TunnelResponse? data = JsonConvert.DeserializeObject<TunnelResponse>(json);
                if (data == null || data?.Tunnels == null) return dictionary;

                foreach (Tunnel tunnel in data.Tunnels)
                {
                    if (string.IsNullOrWhiteSpace(tunnel.Name) || string.IsNullOrWhiteSpace(tunnel.PublicUrl))
                        continue;

                    dictionary[tunnel.Name] = tunnel.PublicUrl;
                }

            }
            catch (JsonException ex)
            {
                Trace.TraceError($"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Unexpected error: {ex.Message}");
            }

            return dictionary;
        }

        private static Dictionary<string, string> GetTunnelNamesAndUrlsFromXML(string xml)
        {
            var dictionary = new Dictionary<string, string>();
            var document = XDocument.Parse(xml);

            var tunnels = document.Descendants("Tunnels");
            foreach (var tunnel in tunnels)
            {
                var name = tunnel.Element("Name")?.Value;
                var publicUrl = tunnel.Element("PublicURL")?.Value;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(publicUrl))
                {
                    dictionary[name] = publicUrl;
                }
            }

            return dictionary;
        }
    }
}
