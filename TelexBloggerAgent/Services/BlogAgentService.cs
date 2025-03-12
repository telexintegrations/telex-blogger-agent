using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _telexWebhookUrl;
        private ILogger<BlogAgentService> _logger;

        public BlogAgentService(IOptions<GeminiSetting> geminiSettings, IOptions<TelexSetting> telexWebhookUrl, ILogger<BlogAgentService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _telexWebhookUrl = telexWebhookUrl.Value.WebhookUrl;
            _logger = logger;
        }

        public async Task<string?> GenerateBlogAsync(GenerateBlogDto blogPrompt)
        {
            // Request body for Gemini api call
            var requestBody = new
            {
                contents = new[]
                {
                    new 
                    { 
                        role = "user", 
                        parts = new[] 
                        { 
                            new 
                            { 
                                text = $"{blogPrompt.Message}. Using a format of Title, Introduction, Body and Conclusion. Also remove any markdown, and just return as a plain string." 
                            } 
                        } 
                    }


                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={_apiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

            var blogResponse = responseJson.GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            _logger.LogInformation("Blog post successfully generated");

            if (string.IsNullOrEmpty(blogResponse))
            {
                _logger.LogInformation("Failed to generate blog post");
                return null;
            }

            // Define the payload for the telex channel
            var payload = new
            {
                event_name = "Blog A.I",
                message = blogResponse,
                status = "success",
                username = "Blogger Agent"
            };

            // Serialize telex payload to JSON
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var telexContent = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");

            var telexWebhookUrl = blogPrompt.Settings
                .Where(s => s.Label == "webhook_url")
                .Select(s => s.Default)
                .FirstOrDefault()
                .ToString();

            if (string.IsNullOrEmpty(telexWebhookUrl))
            {
                throw new Exception("Telex Webhook Url is null");
            }
            var telexResponse = await _httpClient.PostAsync(telexWebhookUrl, telexContent);


            if (telexResponse.IsSuccessStatusCode)
            {
                string responseContent = await telexResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Blog post successfully sent to telex: {responseContent}",responseContent);
                // return responseContent;
                return blogResponse;
                
            }

            return null;
        }
    }
}
