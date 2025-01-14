using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Data.Ngrok
{
    public class NgrokServerConfig : INotifyPropertyChanged
    {
        private string _startupCommand = "ngrok start --all";

        private string _minecraftTunnelId = "minecraft";
        private string _webPanelTunnelId = "web-panel";
        private string _sftpTunnelId = "sftp";

        public string StartupCommand
        {
            get => _startupCommand;
            set => SetAndNotify(ref _startupCommand, value, nameof(StartupCommand));
        }

        public string MinecraftTunnelId
        {
            get => _minecraftTunnelId;
            set => SetAndNotify(ref _minecraftTunnelId, value, nameof(MinecraftTunnelId));
        }
        public string WebPanelTunnelId
        {
            get => _webPanelTunnelId;
            set => SetAndNotify(ref _webPanelTunnelId, value, nameof(WebPanelTunnelId));
        }
        public string SftpTunnelId
        {
            get => _sftpTunnelId;
            set => SetAndNotify(ref _sftpTunnelId, value, nameof(SftpTunnelId));
        }

        private void SetAndNotify<T>(ref T var, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(var, value)) return;

            var = value;
            PropertyChanged?.Invoke(this, new(propertyName));
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
