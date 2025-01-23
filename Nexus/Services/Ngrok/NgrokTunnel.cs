using Nexus.Data.Ngrok;
using Nexus.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.Ngrok
{
    public class NgrokTunnel : INotifyPropertyChanged
    {
        private ServerStatus _serverStatus = ServerStatus.Offline;
        private NgrokTunnelController _ngrokTunnelController;

        public NgrokServerConfig Config { get; } = new();

        private string _minecraftEndpoint = "-";
        private string _webPanelEndpoint = "-";
        private string _sftpEndpoint = "-";

        public string MinecraftEndpoint
        {
            get => _minecraftEndpoint;
            private set
            {
                if (_minecraftEndpoint != value)
                {
                    _minecraftEndpoint = value;
                    PropertyChanged?.Invoke(this, new(nameof(MinecraftEndpoint)));
                }
            }
        }
        public string WebPanelEndpoint
        {
            get => _webPanelEndpoint;
            private set
            {
                if (_webPanelEndpoint != value)
                {
                    _webPanelEndpoint = value;
                    PropertyChanged?.Invoke(this, new(nameof(WebPanelEndpoint)));
                }
            }
        }
        public string SftpEndpoint
        {
            get => _sftpEndpoint;
            private set
            {
                if (_sftpEndpoint != value)
                {
                    _sftpEndpoint = value;
                    PropertyChanged?.Invoke(this, new(nameof(SftpEndpoint)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ServerStatus Status
        {
            get => _serverStatus;
            set
            {
                if (_serverStatus != value)
                {
                    _serverStatus = value;
                    PropertyChanged?.Invoke(this, new(nameof(Status)));
                }
            }
        }
        public int Port { get; private set; } = 4040;

        public NgrokTunnel()
        {
            _ngrokTunnelController = new(Port);
            ApplyEventListener();

            _ngrokTunnelController.Start();
        }

        private void ApplyEventListener()
        {
            _ngrokTunnelController.Disconnected += () =>
            {
                Status = ServerStatus.Offline;

                MinecraftEndpoint = "-";
                WebPanelEndpoint = "-";
                SftpEndpoint = "-";
            };
            _ngrokTunnelController.EndpointsReceived += (endpoints) =>
            {
                Status = ServerStatus.Online;

                SafeSet(ref _minecraftEndpoint, endpoints, Config.MinecraftTunnelId, "-");
                SafeSet(ref _webPanelEndpoint, endpoints, Config.WebPanelTunnelId, "-");
                SafeSet(ref _sftpEndpoint, endpoints, Config.SftpTunnelId, "-");
            };
        }

        private void SafeSet(ref string var, Dictionary<string, string> dict, string key, string _default = "-")
        {
            dict.TryGetValue(key, out var);
            var ??= _default;
        }

        public void Start()
        {
            if (Status != ServerStatus.Offline) return;

            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {Config.StartupCommand}",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting ngrok process: {ex.Message}");
            }
        }
    }
}
