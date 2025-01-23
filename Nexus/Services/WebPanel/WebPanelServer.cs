using Nexus.Data.Ngrok;
using Nexus.Data.WebPanel;
using Nexus.Enum;
using Nexus.Services.Ngrok;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.WebPanel
{
    public class WebPanelServer : INotifyPropertyChanged
    {
        private ServerStatus _serverStatus = ServerStatus.Offline;
        private WebPanelServerController _webPanelServerController;

        public WebPanelServerConfig Config { get; } = new();

        public int Port { get; private set; } = 5280;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        public WebPanelServer()
        {
            _webPanelServerController = new(Port);
            ApplyEventListener();

            _webPanelServerController.Start();
        }

        private void ApplyEventListener()
        {
            _webPanelServerController.Connected += () => Status = ServerStatus.Online;
            _webPanelServerController.Disconnected += () => Status = ServerStatus.Offline;
        }

        public void Start()
        {
            if (Status != ServerStatus.Offline) return;

            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {Config.NexusWebPanelPath}",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting Web Panel process: {ex.Message}");
            }
        }
    }
}
