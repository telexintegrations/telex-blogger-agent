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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessBlog([FromBody] GenerateBlogDto blogDto)
        {
            if (string.IsNullOrEmpty(blogDto.Message) || !blogDto.Settings.Any())
            {
                return BadRequest("Message and settings are required");
            }

            // Get the request origin (where the request is coming from)
            string referer = HttpContext.Request.Headers["Referer"].ToString();
            string origin = HttpContext.Request.Headers["Origin"].ToString();

            _logger.LogInformation($"Telex Referer URL: {referer}");
      
            _logger.LogInformation($"Telex Origin URL: {origin}");
            
            string host = HttpContext.Request.Headers["Host"].ToString();
            string forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string fullUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            string[] pathSegments = HttpContext.Request.Path.Value.Split('/');
            string lastSegment = pathSegments.LastOrDefault();
            string channelIdQuery = HttpContext.Request.Query["channelId"];
            string channelIdHeader = HttpContext.Request.Headers["X-Channel-Id"];
            string originalUrl = HttpContext.Request.Headers["X-Original-URL"];

            foreach (var header in HttpContext.Request.Headers)
            {
                _logger.LogInformation($"Header: {header.Key} = {header.Value}");
            }

            _logger.LogInformation($"Original URL: {originalUrl}");
            _logger.LogInformation($"Extracted Channel ID from URL Path: {lastSegment}");
            _logger.LogInformation($"Extracted Channel ID from Query Params: {channelIdQuery}");
            _logger.LogInformation($"Extracted Channel ID from Headers: {channelIdHeader}");

            _logger.LogInformation($"Telex Host: {host}");
            _logger.LogInformation($"Telex X-Forwarded-For: {forwardedFor}");
            _logger.LogInformation($"Telex IP Address: {ipAddress}");
            _logger.LogInformation($"Full Request URL: {fullUrl}");

            if (string.IsNullOrEmpty(blogDto.ChannelId) && referer != null)
            {

                var channelId = referer.Split('/').LastOrDefault();
                blogDto.ChannelId = channelId;
                _logger.LogInformation("Channel url is null, extracted from URL: " + channelId);
            }

            _logger.LogInformation($"Request received: {JsonSerializer.Serialize(blogDto)}");

            Task.Run(() => _blogService.HandleAsync(blogDto));

            return Ok(blogDto.Message);
        }
      
    }
}
