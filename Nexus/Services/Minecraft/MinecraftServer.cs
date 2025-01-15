using Nexus.Data.Minecraft;
using Nexus.Enum;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Nexus.Services.Minecraft
{
    public class MinecraftServer : INotifyPropertyChanged
    {
        private readonly string _serverControllerPath =
            ".\\Executable\\NexusMinecraftServer\\NexusMinecraftServer.exe";
        private ServerStatus _serverStatus = ServerStatus.Offline;

        private readonly MinecraftWebsocketController _webSocketController;
        private string _clientWebsocketLogs = "";

        public MinecraftServerConfig Config { get; } = new();
        public MinecraftWebsocketController WebsocketController
        {
            get => _webSocketController;
        }
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
        public string WebsocketLogs
        {
            get => _clientWebsocketLogs;
            private set
            {
                if (_clientWebsocketLogs != value)
                {
                    _clientWebsocketLogs = value;
                    PropertyChanged?.Invoke(this, new(nameof(WebsocketLogs)));
                }
            }
        }
        public int WebsocketPort { get; private set; } = 8080;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MinecraftServer()
        {
            _webSocketController = new();
            ApplyWebsocketEventListener();

            _webSocketController.Start();
        }

        private void ApplyWebsocketEventListener()
        {
            _webSocketController.OnConnected += () => Status = ServerStatus.Online;
            _webSocketController.OnDisconnected += () => Status = ServerStatus.Offline;
            _webSocketController.OnMessageReceived += HandleWebSocketMessage;
        }

        private void HandleWebSocketMessage(string message)
        {
            if (message.Contains("[Nexus] Server is running!"))
                Status = ServerStatus.Online;
            else if (message.Contains("[Nexus] Server is not running."))
                Status = ServerStatus.Inactive;
            else WebsocketLogs += message.Trim() + "\n";
        }

        public void Start()
        {
            if (Status != ServerStatus.Offline) return;
            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = _serverControllerPath,
                    Arguments = $"{Config.ServerPath} {Config.Arguments}",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting Minecraft Server Controller: {ex.Message}");
            }
        }

        public void Stop()
        {
            _webSocketController.Stop();
        }
    }
}
