using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Controllers
{
    [Route("api/telex-integration")]
    [ApiController]
    public class TelexIntegrationController : ControllerBase
    {

        private readonly ITelexIngegrationService _integrationService;
        public TelexIntegrationController(ITelexIngegrationService integrationService)
        {
            _integrationService = integrationService;
        }


        [HttpGet]
        public IActionResult GetIntegrationConfig()
        {

            var integrationJson = _integrationService.LoadIntegration();

            if (integrationJson == null)
            {
                return NotFound();
            }

            return Ok(integrationJson);
        }

    }
}
