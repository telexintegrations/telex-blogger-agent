using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (string.IsNullOrEmpty(blogDto.Message))
            {
                return BadRequest("Message is required");
            }
            if (string.IsNullOrEmpty(blogDto.ChannelId))
            {
                return BadRequest("Channel Id is required");
            }

            _logger.LogInformation($"Request received: {blogDto}");

            Task.Run(() => _blogService.HandleAsync(blogDto));

            //return Ok(blogDto.Message);
            return Ok(blogDto.Message);
        }
      
    }
}
