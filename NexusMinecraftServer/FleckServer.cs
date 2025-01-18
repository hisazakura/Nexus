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
        private Fleck.WebSocketServer _server = new($"ws://127.0.0.1:{port}");
        private IWebSocketConnection? _client;
        public int Port { get; } = port;

        public event Action<string>? MessageReceived;

        public void Start()
        {
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Client connected.");
                    _client = socket;
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Client disconnected.");
                    _client = null;
                };
                socket.OnMessage = message =>
                    MessageReceived?.Invoke(message);
            });
        }

        public void Stop()
        {
            _server.Dispose();
        }

        public void SendMessage(string input)
        {
            _client?.Send(input);
        }
    }
}
