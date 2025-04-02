using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Dtos;
using Microsoft.AspNetCore.Identity;

namespace TelexBloggerAgent.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        const string identifier = "📝 #TelexBlog"; // Identifier
        const string topicIdentifier = "suggest topics topic";
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

        public async Task HandleBlogRequestAsync(GenerateBlogDto blogPrompt)
        {
            if (blogPrompt.Message.Contains(identifier))
            {
                _logger.LogInformation("Telex message contains identifier. Skipping API call to prevent loop.");
                return;
            }

            try
            {
                if (blogPrompt.Message.Contains(topicIdentifier)) 
                {
                    _logger.LogInformation("User requested blog topic suggestions.");
                    await SuggestTopicsAsync(blogPrompt);
                } else
                {
                    _logger.LogInformation("Generating full blog post.");
                    await GenerateBlogAsync(blogPrompt);

                }

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process blog event");
            }
        }

        public async Task RefineContentAsync(RefineBlogDto blogPrompt)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(blogPrompt.Message))
                {
                    throw new ArgumentException("Blog content is required.");
                }

                if (string.IsNullOrEmpty(blogPrompt.RefinementInstructions))
                {
                    throw new ArgumentException("Refinement instructions are required.");
                }

                // Load settings from Integration.json
                var integrationSettings = LoadIntegrationSettings();

                // Prepare the refinement prompt for Gemini
                var refinementPrompt = $"Refine the following blog content based on the user's request: {blogPrompt.Message}. " +
                                      $"Make the following changes: {blogPrompt.RefinementInstructions}. " +
                                      $"Tone: {integrationSettings.Tone}. " +
                                      $"Blog Length: {integrationSettings.BlogLength}. " +
                                      $"Format: {integrationSettings.Format}. " +
                                      "Return the refined content as plain text without markdown formatting.";

                // Create the request body for Gemini
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
                            text = refinementPrompt
                        }
                    }
                }
            }
                };

                // Serialize the request body to JSON
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                _logger.LogInformation("Refining blog content...");

                // Call the Gemini API
                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse the Gemini API response
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

                var refinedContent = responseJson.GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (refinedContent == null)
                {
                    throw new Exception("Failed to refine blog content");
                }

                _logger.LogInformation("Blog content refined successfully");

                // Send the refined content to the user
                await SendBlogAsync(refinedContent, blogPrompt.Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refine blog content");
                throw;
            }
        }

        private dynamic LoadIntegrationSettings()
        {
            var json = File.ReadAllText("Integration.json");
            var doc = JsonDocument.Parse(json);
            var settings = doc.RootElement.GetProperty("data").GetProperty("settings");

            return new
            {
                Tone = settings.EnumerateArray().First(s => s.GetProperty("label").GetString() == "tone").GetProperty("default").GetString(),
                BlogLength = settings.EnumerateArray().First(s => s.GetProperty("label").GetString() == "blog_length").GetProperty("default").GetString(),
                Format = settings.EnumerateArray().First(s => s.GetProperty("label").GetString() == "format").GetProperty("default").GetString()
            };
        }


        public async Task SuggestTopicsAsync(GenerateBlogDto blogPrompt)
        {
            try
            {
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
                                    text = $"Suggest 5 to 10 engaging blog post topics related to: {blogPrompt.Message}. Return the topics in bullet points, use ✅ for bullet points. Return the content as plain text without markdown formatting."
                                 }
                            }
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _logger.LogInformation("Requesting blog topic suggestions...");
                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

                var topics = responseJson.GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();



                if (topics == null)
                {
                    throw new Exception("Failed to retrieve blog topics");
                } 
                else
                {
                    _logger.LogInformation("Blog topics generated successfully");
                    await SendBlogAsync(topics, blogPrompt.Settings);
                }

            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate blog topics");
            }
        }
    }
}
