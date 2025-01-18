using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusMinecraftServer
{
    public class WebSocketServerCollection
    {
        public List<WebSocketServer> WebSocketControllers { get; private set; } = [];
        public event Action<WebSocketServer, string>? MessageReceived;
        
        private void ApplyListeners(WebSocketServer controller)
        {
            controller.MessageReceived += 
                (message) => MessageReceived?.Invoke(controller, message);
        }

        public WebSocketServer Create(int port)
        {
            WebSocketServer controller = new(port);
            WebSocketControllers.Add(controller);
            ApplyListeners(controller);
            return controller;
        }

        public bool TryGet(int port, out WebSocketServer? controller)
        {
            controller = null;
            List<WebSocketServer> foundControllers = 
                WebSocketControllers.FindAll(controller => controller.Port == port);
            if (foundControllers.Count != 0)
            {
                controller = foundControllers.First();
                return true;
            }
            return false;
        }

        public void Broadcast(string message)
        {
            foreach (WebSocketServer webSocketController in WebSocketControllers)
                webSocketController.SendMessage(message);
        }

        public void Stop()
        {
            foreach (WebSocketServer webSocketController in WebSocketControllers)
                webSocketController.Stop();
        }
    }
}
