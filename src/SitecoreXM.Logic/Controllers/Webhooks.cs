using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

namespace SitecoreXM.Logic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Webhooks : ControllerBase
    {
        private readonly ILogger _logger;

        public Webhooks(ILogger<Webhooks> logger)
        {
            _logger = logger;
        }

        [HttpPost("Debug")]
        public string Debug([FromBody] JsonObject value)
        {
            _logger.LogInformation($"Received:\n {value}");
            return "OK";
        }
    }
}
