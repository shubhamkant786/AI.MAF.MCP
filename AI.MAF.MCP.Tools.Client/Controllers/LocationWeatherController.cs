using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;

namespace AI.MAF.MCP.Tools.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationWeatherController : ControllerBase
    {

        [HttpGet("getWeatherForecast")]
        public async Task<string> Get([FromKeyedServices("mcpToolAgent")] AIAgent agent,
            [FromQuery] string queryMessage)
        {
            var result = await agent.RunAsync(queryMessage);
            return result.Text;
        }
    }
}
