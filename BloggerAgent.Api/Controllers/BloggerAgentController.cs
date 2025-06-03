using BloggerAgent.Application.Dtos;
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
        public async Task<IActionResult> ProcessBlog([FromBody] TaskRequest request)
        {           

            _logger.LogInformation($"Request received: {JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })}");

            var response = await _blogService.HandleAsync(request);

            return Ok(response);
        }

        [HttpPost("task/get{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTask([FromBody] string id)
        {
            return Ok();
        }
    }
}
