using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BloggerAgent.Application.IServices;
using BloggerAgent.Application.Helpers;

namespace BloggerAgent.Api.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TelexIntegrationController : ControllerBase
    {       

        [HttpGet(".well-known/agent.json")]
        public IActionResult GetIntegrationConfig()
        {

            var integrationJson = AgentSpec.GetAgentCard();

            if (integrationJson == null)
            {
                return NotFound();
            }

            return Ok(integrationJson);
        }

    }
}
