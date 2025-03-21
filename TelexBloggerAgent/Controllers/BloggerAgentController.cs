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

            Task.Run(() => _blogService.HandleBlogRequestAsync(blogDto));

            return Ok(blogDto.Message);
        }
        /// <summary>
        /// "This method refines already generated blog"
        /// </summary>
        /// <param name="blogPrompt"></param>
        /// <returns></returns>
        [HttpPost("refine-blog")]
        public async Task<IActionResult> RefineContent([FromBody] RefineBlogDto blogPrompt)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(blogPrompt.Message))
                {
                    return BadRequest("Blog content is required.");
                }

                if (string.IsNullOrEmpty(blogPrompt.RefinementInstructions))
                {
                    return BadRequest("Refinement instructions are required.");
                }

                // Call the refinement method
                await _blogService.RefineContentAsync(blogPrompt);
                return Ok("Content refined successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
