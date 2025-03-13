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
        private readonly string _geminiUrl;
        private ILogger<BlogAgentService> _logger;

        public BlogAgentService(IOptions<GeminiSetting> geminiSettings, ILogger<BlogAgentService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _logger = logger;
        }


        private string FormatBlogPrompt(string userPrompt)
        {
            return $"{userPrompt}. Ensure the response is a well-structured, engaging, and informative article. " +
                   "Start with an attention-grabbing opening, provide valuable insights in a natural flow, " +
                   "and end with a compelling conclusion. Keep the content clear and concise, and return it as plain text.";
        }

        public async Task GenerateBlogAsync(GenerateBlogDto blogPrompt)
        {
            try
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
                                    text = FormatBlogPrompt(blogPrompt.Message)
                                } 
                            } 
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);
                _logger.LogInformation("Blog post successfully generated");

                var blogPost = responseJson.GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                await SendBlogAsync(blogPost, blogPrompt.Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate blog post");
            }
        }

        public async Task SendBlogAsync(string blogPost, List<Setting> settings)
        {
            if (string.IsNullOrEmpty(blogPost))
            {
                _logger.LogInformation("Failed to generate blog post");
                return;
            }

            // Define the payload for the telex channel
            var payload = new
            {
                event_name = "Blog AI",
                message = blogPost,
                status = "success",
                username = "Blogger Agent"
            };

            // Serialize telex payload to JSON
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var telexContent = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");

            var telexWebhookUrl = settings
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
                _logger.LogInformation("Blog post successfully sent to telex: {responseContent}", responseContent);
            }
        }
    }
}
