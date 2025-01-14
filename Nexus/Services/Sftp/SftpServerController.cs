using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.Sftp
{
    public class SftpServerController(string host, int port, string username, string password)
    {
        private readonly string _host = host;
        private readonly int _port = port;

        private string _username = username;
        private string _password = password;

        private SftpClient? _client;
        private Task? _monitorTask;
        private CancellationTokenSource _cancellationTokenSource = new();

        public event Action? Connected;
        public event Action? Disconnected;

        public void UpdateUser(string username, string password)
        {
            _username = username;
            _password = password;

            _client?.Dispose();
            _client = new(_host, _port, _username, _password);
        }

        public void Start()
        {
            if (_monitorTask != null) return;

            _monitorTask = Task.Run(MonitorSftpPort);
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

        private async Task MonitorSftpPort()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _client ??= new(_host, _port, _username, _password);

                    await _client.ConnectAsync(_cancellationTokenSource.Token);

                    if (_client.IsConnected)
                    {
                        Connected?.Invoke();
                        _client.Disconnect();
                    }
                    else Disconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error: {ex.Message}");
                    Disconnected?.Invoke();
                }

                await Task.Delay(2000, _cancellationTokenSource.Token);
            }
        }
    }
}
