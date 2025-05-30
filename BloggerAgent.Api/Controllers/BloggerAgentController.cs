﻿using BloggerAgent.Application.Dtos;
using BloggerAgent.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace BloggerAgent.Api.Controller
{
    [Route("api/v1/blogger-agent")]
    [ApiController]
    public class BloggerAgentController : ControllerBase
    {
        private readonly IBlogAgentService _blogService;
        private ILogger<BloggerAgentController> _logger;
        public BloggerAgentController(IBlogAgentService blogService, ILogger<BloggerAgentController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        /// <summary>
        /// Enter text
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessBlog([FromBody] GenerateBlogDto blogDto)
        {
            if (string.IsNullOrEmpty(blogDto.Message))
            {
                return BadRequest("Message is required");
            }           

            if (string.IsNullOrEmpty(blogDto.ChannelId))
            {                               
                _logger.LogInformation("Channel ID is null");
                return BadRequest("Channel ID is required");
            }
            
            if (string.IsNullOrEmpty(blogDto.OrganizationId))
            {                               
                _logger.LogInformation("Organization ID is null");
                return BadRequest("Organization ID is required");
            }

            _logger.LogInformation($"Request received: {JsonSerializer.Serialize(blogDto, new JsonSerializerOptions { WriteIndented = true })}");

            string response = await _blogService.HandleAsync(blogDto);

            return Ok(response);
        }
      
    }
}
