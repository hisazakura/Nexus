using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class FleckServer(int port)
    {
        private readonly Fleck.WebSocketServer _server = new($"ws://127.0.0.1:{port}");
        private readonly List<IWebSocketConnection> _clients = [];
        public int Port { get; } = port;

        public event Action<IWebSocketConnection>? ClientConnected;
        public event Action<IWebSocketConnection>? ClientDisonnected;
        public event Action<IWebSocketConnection, string>? MessageReceived;

        private void ApplyListeners(IWebSocketConnection socket)
        {
            socket.OnOpen = () =>
            {
                _clients.Add(socket);
                ClientConnected?.Invoke(socket);
            };
            socket.OnClose = () => { 
                _clients.Remove(socket);
                ClientDisonnected?.Invoke(socket);
            };
            socket.OnMessage = message => { 
                MessageReceived?.Invoke(socket, message);
            };
        }

        public void Start()
        {
            _server.Start(ApplyListeners);
        }

        public void Stop()
        {
            _server.Dispose();
        }

        public void SendMessage(string message, Guid id)
        {
            _clients.Find(client => client.ConnectionInfo.Id == id)?.Send(message);
        }

        public void Broadcast(string message)
        {
            foreach (IWebSocketConnection client in _clients)
                client.Send(message);
        }
    }
}
