using System.Text;
using System.Text.Json;

namespace TelexBloggerAgent.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            var request = context.Request;
            var headers = JsonSerializer.Serialize(request.Headers.ToDictionary(headers => headers.Key, headers => headers.Value.ToString()));
            var queryParams = JsonSerializer.Serialize(request.Query.ToDictionary(query => query.Key, query => query.Value.ToString()));
            
            string body = "";
            if (request.Body.CanRead)
            {
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Reset the stream for next middleware
                }
            }

            _logger.LogInformation($"🔍 Incoming Request: {request.Method} {request.Path}");
            _logger.LogInformation($"📌 Headers: {headers}");
            _logger.LogInformation($"📌 Query Parameters: {queryParams}");
            _logger.LogInformation($"📌 Body: {body}");

            await _next(context);
        }
    }
}
