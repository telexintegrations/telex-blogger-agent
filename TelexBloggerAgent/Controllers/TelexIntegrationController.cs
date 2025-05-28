using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Controllers
{
    [Route("api/v1/telex-integration")]
    [ApiController]
    public class TelexIntegrationController : ControllerBase
    {

        private readonly ITelexIntegrationService _integrationService;
        public TelexIntegrationController(ITelexIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }


        [HttpGet]
        public IActionResult GetIntegrationConfig()
        {

            var integrationJson = Agent_A2A.GetAgentA2A();
            //var integrationJson = _integrationService.LoadIntegration();

            if (integrationJson == null)
            {
                return NotFound();
            }

            return Ok(integrationJson);
        }

    }
}
