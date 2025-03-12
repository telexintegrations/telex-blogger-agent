using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloggerAgentController : ControllerBase
    {
        private readonly IBlogAgentService _blogService;

        public BloggerAgentController(IBlogAgentService blogService)
        {
            _blogService = blogService;
        }

        /// <summary>
        /// Enter text
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GenerateBlogDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessBlog([FromBody] GenerateBlogDto blogDto)
        {
            var blogDraft = await _blogService.GenerateBlogAsync(blogDto.Message);

            if (blogDraft == null)
            {
                return BadRequest(blogDraft);
            }
            return Ok(blogDraft);
        }
                        
    }
}
