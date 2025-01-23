using Microsoft.AspNetCore.Mvc;

namespace NexusWebPanel.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class BackendController : Controller
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "ok" });
        }
    }
}
