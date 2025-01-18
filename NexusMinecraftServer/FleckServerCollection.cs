using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class FleckServerCollection
    {
        public List<FleckServer> FleckServers { get; private set; } = [];
        public event Action<FleckServer, string>? MessageReceived;

        private void ApplyListeners(FleckServer server)
        {
            server.MessageReceived +=
                (message) => MessageReceived?.Invoke(server, message);
        }

        public FleckServer Create(int port)
        {
            FleckServer server = new(port);
            FleckServers.Add(server);
            ApplyListeners(server);

            server.Start();
            return server;
        }

        public bool TryGet(int port, out FleckServer? controller)
        {
            controller = null;
            List<FleckServer> foundControllers =
                FleckServers.FindAll(server => server.Port == port);
            if (foundControllers.Count != 0)
            {
                controller = foundControllers.First();
                return true;
            }
            return false;
        }

        public void Broadcast(string message)
        {
            foreach (FleckServer server in FleckServers)
                server.SendMessage(message);
        }

        public void Stop()
        {
            foreach (FleckServer server in FleckServers)
                server.Stop();
        }
    }
}
