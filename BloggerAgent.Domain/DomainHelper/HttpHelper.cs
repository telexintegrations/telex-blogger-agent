using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using BloggerAgent.Domain.Commons.DataEntities;
using Microsoft.Extensions.Logging;

namespace BloggerAgent.Domain.DomainHelper
{
    public class HttpHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpHelper> _logger;

        public HttpHelper(IHttpClientFactory httpClientFactory, ILogger<HttpHelper> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendRequestAsync(ApiRequest request)
        {

            HttpRequestMessage httpRequest = new HttpRequestMessage(request.Method, request.Url);

            // Add headers
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                _logger.LogInformation($"[HttpHelper] Request Headers:\n{request.Headers}");

            }

            // Add body if present
            if (request.Body != null)
            {
                string json = JsonSerializer.Serialize(request.Body, new JsonSerializerOptions 
                { 
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true, 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"[HttpHelper] Request Body:\n{json}");
            }

            _logger.LogInformation($"[HttpHelper] Sending {request.Method} request to {request.Url}");

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);

                // Log response info
                var headers = string.Join("\n", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));
                var statusCode = (int)response.StatusCode;
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("[HttpHelper] Received {StatusCode} from {Url}", (int)response.StatusCode, request.Url);
                _logger.LogInformation($"[HttpHelper] Response Headers:\n{headers}");
                _logger.LogInformation("[HttpHelper] Response body:\n{Body}", responseBody);
              
                return response;
            }           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending HTTP request to {Url}", request.Url);

                throw new Exception($"An error occured while sending http request: {ex.Message}", ex);
            }
        }
    }
}
