using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace NexusWebPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TunnelsController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly string _tunnelsUrl = "http://127.0.0.1:4040/api/tunnels";

        [HttpGet]
        public async Task<IActionResult> GetTunnels()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_tunnelsUrl);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                JsonDocument data = JsonDocument.Parse(content);

                JsonElement tunnels = data.RootElement.GetProperty("tunnels");
                Dictionary<string, string> result = [];

                foreach (JsonElement tunnel in tunnels.EnumerateArray())
                {
                    string? publicUrl = tunnel.GetProperty("public_url").GetString();
                    if (publicUrl == null) continue;

                    // Extract the port
                    string? configAddr = tunnel.GetProperty("config").GetProperty("addr").GetString();
                    if (configAddr == null) continue;

                    string port = configAddr.Split(':').Last();

                    // Add the processed result
                    result.Add(port, publicUrl);
                }

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching data from {_tunnelsUrl}: {ex.Message}");
            }
        }
    }
}
