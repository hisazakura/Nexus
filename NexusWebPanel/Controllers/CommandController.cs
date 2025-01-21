using Microsoft.AspNetCore.Mvc;
using NexusWebPanel.Services;

namespace NexusWebPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController(WebsocketListener listener) : ControllerBase
    {
        private readonly WebsocketListener _listener = listener;

        [HttpPost]
        public IActionResult SendCommand([FromBody] CommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Command))
                return BadRequest(new { error = "Command cannot be empty" });

            // Send the command using the WebsocketListener
            _listener.SendMessage(request.Command);

            return Ok(new { message = "Command sent successfully" });
        }

        public class CommandRequest
        {
            public string? Command { get; set; }
        }
    }
}
