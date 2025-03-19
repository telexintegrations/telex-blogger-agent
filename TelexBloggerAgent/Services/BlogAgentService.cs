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
        const string identifier = "📝 #TelexBlog"; // Identifier
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _geminiUrl;
        private ILogger<BlogAgentService> _logger;

        public BlogAgentService(IHttpClientFactory httpClientFactory, IOptions<GeminiSetting> geminiSettings, ILogger<BlogAgentService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _logger = logger;
        }

        private string GetSettingValue(List<Setting> settings, string key)
        {
            return settings.FirstOrDefault(s => s.Label == key)?.Default.ToString() ?? "";
        }

        private string FormatBlogPrompt(string userPrompt, List<Setting> settings)
        {
            // Retrieve settings dynamically
            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string companyWebsite = GetSettingValue(settings, "company_website");
            string tone = GetSettingValue(settings, "tone");
            string blogLength = GetSettingValue(settings, "blog_length");
            string format = GetSettingValue(settings, "format");

            // Base prompt structure
            string prompt = $"{userPrompt}. Ensure the response is a well-structured, engaging, and informative article.";

            // Adjust tone dynamically
            prompt += tone switch
            {
                "Professional" => " Use a formal and authoritative tone suitable for industry professionals.",
                "Casual" => " Use a conversational and friendly tone to keep the content engaging.",
                "Persuasive" => " Craft the content in a marketing-focused way, encouraging action and conversions.",
                "Informative" => " Keep the content objective, educational, and easy to understand.",
                _ => ""
            };

            // Incorporate company branding if provided
            if (!string.IsNullOrWhiteSpace(companyName) && !string.IsNullOrWhiteSpace(companyOverview))
            {
                prompt += $" Align the content with the company, {companyName}; {companyOverview}.";
            }

            // Adjust content length
            prompt += blogLength switch
            {
                "short" => " Keep the article concise, around 300 words, focusing on key points.",
                "medium" => " Provide a balanced article, around 600 words, with in-depth insights.",
                "long" => " Create a comprehensive article, around 1000+ words, with detailed analysis.",
                _ => ""
            };

            // Format preference
            prompt += format switch
            {
                "Outline" => " Return the response as a structured outline with key points.",
                "Summary" => " Summarize the content concisely, highlighting key takeaways.",
                "Full Article" => " Structure the response with a title, introduction, body, and conclusion, but do not explicitly categorize them.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(companyWebsite))
            {
                prompt += $" Add a call to action in the conclusion of the article. Use the raw link to the company website: {companyWebsite}";
            }


            // Formatting preferences
            prompt += " Use ALL CAPS for section headers and important words, and use ✅ for bullet points.";
            prompt += " Return the content as plain text without markdown formatting.";

            return prompt;
        }


        public async Task GenerateBlogAsync(GenerateBlogDto blogPrompt)
        {
            if (blogPrompt.Message.Contains(identifier))
            {
                _logger.LogInformation("Telex message contains identifier. Skipping API call to prevent loop.");
                return;
            }

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
                                    text = FormatBlogPrompt(blogPrompt.Message, blogPrompt.Settings)
                                } 
                            } 
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                _logger.LogInformation("Generating blog post.....");

                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

                var blogPost = responseJson.GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (blogPost == null)
                {
                    throw new Exception("Failed to generate blog post");
                }

                _logger.LogInformation("Blog post generated successfully");

                await SendBlogAsync(blogPost, blogPrompt.Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate blog post");
                throw;
            }
        }

        public async Task SendBlogAsync(string blogPost, List<Setting> settings)
        {
            var signedBlogPost = $"{blogPost}\n\n{identifier}";
           
            // Define the payload for the telex channel
            var payload = new
            {
                event_name = "Blog AI",
                message = signedBlogPost,
                status = "success",
                username = "Blogger Agent"
            };

            // Serialize telex payload to JSON
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var telexContent = new StringContent(jsonPayload, new UTF8Encoding(false), "application/json");

            var telexWebhookUrl = settings                
                .FirstOrDefault(s => s.Label == "webhook_url")?.Default
                .ToString();

            if (string.IsNullOrEmpty(telexWebhookUrl))
            {
                throw new Exception("Telex Webhook Url is null");
            }

            var telexResponse = await _httpClient.PostAsync(telexWebhookUrl, telexContent);

            if ((int)telexResponse.StatusCode == StatusCodes.Status202Accepted)
            {
                string responseContent = await telexResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Blog post successfully sent to telex");
            }
        }
    }
}
