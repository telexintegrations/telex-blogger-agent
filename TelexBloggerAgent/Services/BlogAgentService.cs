﻿using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Dtos.GeminiDto;

namespace TelexBloggerAgent.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        private static readonly List<ChatMessage> conversations = new();

        const string identifier = "📝 #TelexBlog"; // Identifier
        private readonly HttpClient _httpClient;
        private ILogger<BlogAgentService> _logger;
        private readonly string _apiKey;
        private readonly string _geminiUrl;
        private readonly IRequestProcessingService _requestService;


        public BlogAgentService(IHttpClientFactory httpClientFactory, IOptions<GeminiSetting> geminiSettings, ILogger<BlogAgentService> logger, IRequestProcessingService requestService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _requestService = requestService;
            _logger = logger;
        }

       

        public async Task<string> HandleAsync(GenerateBlogDto blogPrompt)
        {
            // Check if the message contains the identifier to prevent a loop
            if (blogPrompt.Message.Contains(identifier))
            {
                _logger.LogInformation("Identifier detected. Skipping API call to prevent loop.");
                return null;
            }

            try
            {                               
                
                // Format the blog prompt based on user input and settings
                var request = await _requestService.ProcessUserInputAsync(blogPrompt);
                
                // Generate the blog post using the formatted message
                var aiResponse = await GenerateResponse(request.UserPrompt, request.SystemMessage);

                // Throw an exception if the blog post generation failed
                if (string.IsNullOrEmpty(aiResponse))
                {
                    throw new Exception("Failed to generate response");
                }

                // Append the identifier to the generated blog post
                var signedResponse = $"{aiResponse}\n\n{identifier}";

                // Send the generated blog post to Telex
                var suceeded = await SendBlogAsync(signedResponse, blogPrompt.Settings);

                // Throw an exception if sending the blog post failed
                if (!suceeded)
                {
                    throw new Exception("Failed to send blog post to Telex");
                }
                return signedResponse;

            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Failed to generate blog post");
                throw;
            }
        }

        public async Task<string> GenerateResponse(string message, string systemMessage)
        {

            var userMessage = new ChatMessage
            {
                Role = "user",
                Parts = { new Part { Text = message } }
            };

            conversations.Add(userMessage);

            // Create the request body for the Gemini API call
            var requestBody = new GeminiRequest
            {
                SystemInstruction = new SystemMessage
                {
                    Parts = { Text = systemMessage } 
                },
                Contents = conversations
               
            };
            // Serialize the request body to JSON
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            _logger.LogInformation("Generating blog post.....");

            // Send the request to the Gemini API
            var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("An error occured while sending the message to the AI");
                throw new Exception($"An error occured : {response}");
            }

           

            var responseString = await response.Content.ReadAsStringAsync();
            // Deserialize the response from the Gemini API
            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Check if the response contains candidates
            if (!responseJson.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                _logger.LogWarning("No candidates found in the response.");
                throw new Exception("Invalid API response: No candidates.");
            }

            // Extract the generated response from the first candidate
            var generatedResponse = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            // Check if the generated response is null
            if (generatedResponse == null)
            {
                throw new Exception("Failed to generate response");
            }

            _logger.LogInformation("Response generated successfully");

            // Add the generated response to the request body contents
            var modelResponse = new ChatMessage()
            {
                Role = "model",
                Parts = { new Part { Text = generatedResponse } }
            };

            conversations.Add(modelResponse);

            // Return the generated response
            return generatedResponse;
        }

        public async Task<bool> SendBlogAsync(string blogPost, List<Setting> settings)
        {

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
                .FirstOrDefault(s => s.Label == "webhook_url")?.Default
                .ToString();

            // Throw an error if telex webhook url is empty
            if (string.IsNullOrEmpty(telexWebhookUrl))
            {
                throw new Exception("Telex Webhook Url is null");
            }

            // Send the response to telex
            var telexResponse = await _httpClient.PostAsync(telexWebhookUrl, telexContent);

            if ((int)telexResponse.StatusCode == StatusCodes.Status202Accepted)
            {
                string responseContent = await telexResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Blog post successfully sent to telex");

                return true;
            }

            return false;
        }
    }
}
