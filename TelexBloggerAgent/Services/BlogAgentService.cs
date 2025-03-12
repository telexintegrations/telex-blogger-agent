using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _telexWebhookUrl;

        public BlogAgentService(IOptions<GeminiSetting> geminiSettings, IOptions<TelexSetting> telexWebhookUrl)
        {
            _httpClient = new HttpClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _telexWebhookUrl = telexWebhookUrl.Value.WebhookUrl;
        }

        public async Task<string?> GenerateBlogAsync(string blogPrompt)
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
                                text = $"{blogPrompt}. Using a format of Title, Introduction, Body and Conclusion. Also remove any markdown, and just return as a plain string." 
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

            if (string.IsNullOrEmpty(blogResponse))
            {
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

            var telexResponse = await _httpClient.PostAsync(_telexWebhookUrl, telexContent);

            if (telexResponse.IsSuccessStatusCode)
            {
                string responseContent = await telexResponse.Content.ReadAsStringAsync();                

                // return responseContent;
                return blogResponse;
                
            }

            return null;
        }
    }
}
