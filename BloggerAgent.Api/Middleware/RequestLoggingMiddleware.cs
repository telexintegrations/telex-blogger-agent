using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BloggerAgent.Api.Middleware
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
            try
            {
                HttpRequest request = context.Request;
                request.EnableBuffering();

                string headersJson = SerializeAsIndentedJson(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
                string queryJson = SerializeAsIndentedJson(request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

                string body = await ReadRequestBodyAsync(request);
                string formattedBody = body;

                

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        formattedBody = PrettifyJson(body);
                        if (context.Request.Path == "/api/v1/blogger-agent" && context.Request.Method == "POST")
                        {
                            var requestBody = JsonSerializer.Deserialize<TaskRequest>(body, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            var taskContext = DataExtract.ExtractTaskData(requestBody); // your helper method


                            if (taskContext != null)
                            {
                                context.Items["TaskContext"] = taskContext;
                            }

                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "❌ JSON Parsing Error: Malformed request body.");
                    }
                }

                _logger.LogInformation("Request logging Middleware");
                _logger.LogInformation("Incoming Request: {Method} {Path}", request.Method, request.Path);
                _logger.LogInformation("Headers: {Headers}", headersJson);
                _logger.LogInformation("Query Parameters: {Query}", queryJson);
                _logger.LogInformation("Body: {Body}", formattedBody);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception in RequestLoggingMiddleware");
                throw;
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private static string SerializeAsIndentedJson(object data) =>
            JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        private static string PrettifyJson(string rawJson)
        {
            using JsonDocument doc = JsonDocument.Parse(rawJson);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
