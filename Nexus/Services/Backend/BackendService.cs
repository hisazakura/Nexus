using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Services.Backend
{
    public class BackendService
    {
        private readonly HttpListener _listener = new();
        private Task? _listenerTask;
        private CancellationTokenSource _cancellationTokenSource = new();

        public BackendService()
        {
            _listener.Prefixes.Add("http://localhost:1313/");
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                _listenerTask = Task.Run(ListenPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting BackendService: {ex.Message}");
                Stop();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private async Task ListenPort()
        {
            _listener.Start();

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();

                string method = context.Request.HttpMethod;
                string? url = context.Request.RawUrl;

                var response = context.Response;
                var responseString = "<html><head><title>Backend Service</title></head><body>Welcome to the Backend Service</body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            string method = context.Request.HttpMethod;
            string? url = context.Request.RawUrl;

            // Example: Respond to a GET request
            if (method == "GET" && url == "/api/example")
            {
                string responseText = "{\"message\": \"Hello, world!\"}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;
                await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                // Respond with 404 for unknown endpoints
                context.Response.StatusCode = 404;
                byte[] responseBytes = Encoding.UTF8.GetBytes("{\"error\": \"Not found\"}");
                await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }

            context.Response.Close();
        }

        private string GetServerConfig()
        {
            throw new NotImplementedException();
        }
    }
}
