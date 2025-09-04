using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.DomainHelper;
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
        private readonly TaskContextAccessor _taskContextAccessor;
        private ILogger<BloggerAgentController> _logger;

        public BloggerAgentController(IBlogAgentService blogService, ILogger<BloggerAgentController> logger, TaskContextAccessor taskContextAccessor)
        {
            _blogService = blogService;
            _logger = logger;
            _taskContextAccessor = taskContextAccessor;
        }

        /// <summary>
        /// Enter text
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessBlog([FromBody] A2aTaskRequest request)
        {           

            _logger.LogInformation($"Task processing started for task {request.Id}");

            ValidationHelper.ValidateRequest(request);

            var contextSnapshot = _taskContextAccessor.GetTaskContext();

            Task.Run(() => 
            {
                _logger.LogInformation($"Processing task {request.Id} in background");
                _taskContextAccessor.SetTaskContext(contextSnapshot);
                _blogService.HandleUserInput(request);
            });
            _logger.LogInformation($"Task {request.Id} submitted");
            var response = DataExtract.ConstructTaskReceivedResponse(request);

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
