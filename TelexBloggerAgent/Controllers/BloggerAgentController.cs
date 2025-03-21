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
            if (string.IsNullOrEmpty(blogDto.Message) || blogDto.Settings == null)
            {
                return BadRequest("Message or Settings cannot be null");
            }

            _logger.LogInformation($"{blogDto}");
            var response = await _blogService.GenerateResponse(blogDto.Message);
            //Task.Run(() => _blogService.HandleAsync(blogDto));

            //return Ok(blogDto.Message);
            return Ok(response);
        }
                        
    }
}
