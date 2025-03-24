using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Controllers
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
        [HttpPost("generate-blog")]
        [ProducesResponseType(typeof(GenerateBlogDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessBlog([FromBody] GenerateBlogDto blogDto)
        {
            if (string.IsNullOrEmpty(blogDto.Message) || !blogDto.Settings.Any())
            {
                return BadRequest("Message and settings are required");
            }

            // Get the request origin (where the request is coming from)
            string referer = HttpContext.Request.Headers["Referer"].ToString();
            string origin = HttpContext.Request.Headers["Origin"].ToString();

            if (!string.IsNullOrEmpty(referer))
            {
                _logger.LogInformation($"Telex Referer URL: {referer}");
            }
            else if (!string.IsNullOrEmpty(origin))
            {
                _logger.LogInformation($"Telex Origin URL: {origin}");
            }

            var channelId = referer.Split('/').LastOrDefault();

            if (string.IsNullOrEmpty(blogDto.ChannelId))
            {
                blogDto.ChannelId = channelId;
                _logger.LogInformation("Channel url is null, extracted from URL: " + channelId);
            }

            _logger.LogInformation($"Request received: {JsonSerializer.Serialize(blogDto)}");

            Task.Run(() => _blogService.HandleAsync(blogDto));

            return Ok(blogDto.Message);
        }
      
    }
}
