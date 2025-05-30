﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Api.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TelexIntegrationController : ControllerBase
    {

        private readonly ITelexIntegrationService _integrationService;
        public TelexIntegrationController(ITelexIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }


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
