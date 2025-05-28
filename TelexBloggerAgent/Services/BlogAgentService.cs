using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Dtos.GeminiDto;
using System.Threading.Channels;
using System.Collections.Concurrent;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        //private static readonly ConcurrentDictionary<string, List<ChatMessage>> conversations = new(); // Group messages by channelId

        const string identifier = "📝 #TelexBlog"; // Identifier
        private readonly HttpClient _httpClient;
        private ILogger<BlogAgentService> _logger;
        private string _webhookUrl;
        private readonly string _apiKey;
        private readonly string _geminiUrl;
        private readonly IRequestProcessingService _requestService;
        private readonly IConversationService _conversationService;


        public BlogAgentService(
            IHttpClientFactory httpClientFactory, 
            IOptions<GeminiSetting> geminiSettings, 
            IOptions<TelexSetting> telexSettings, 
            ILogger<BlogAgentService> logger, 
            IRequestProcessingService requestService,
            IConversationService conversationService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _webhookUrl = telexSettings.Value.WebhookUrl;
            _requestService = requestService;
            _logger = logger;
            _conversationService = conversationService;
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
                
                // Generate the response using the formatted message
                var aiResponse = await GenerateResponse(request.UserPrompt, request.SystemMessage, blogPrompt.ChannelId);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    throw new Exception("Failed to generate response");
                }

                // Append the identifier to the generated agent response
                var signedResponse = $"{aiResponse}\n\n{identifier}";

                // Send the generated blog post to Telex
                var suceeded = await SendResponseAsync(signedResponse, blogPrompt.ChannelId);

                
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

        public async Task<string> GenerateResponse(string message, string systemMessage, string channelId)
        {
            var conversation = await _conversationService.GetUserConversationsAsync(channelId);
            
            // Create a new conversation and add the message
            if (conversation == null)
            {
               conversation = await _conversationService.StartConversationAsync(channelId, "user", message);
            }
            else
            {
                // Add the message
                await _conversationService.AddMessageAsync(conversation.Id, message, "user");

                conversation.Messages.Add(new Message() { Role = "user", Content = message });
            }                  

            // Prepare the AI request body
            var requestBody = new GeminiRequest
            {
                SystemInstruction = new SystemMessage
                {
                    Parts = { Text = systemMessage }
                },
                //Contents = conversations[channelId] // Send only messages for this channel
                Contents = conversation.Messages.Select(c => new ChatMessage()
                {
                    Role = c.Role,
                    Parts = { new Part { Text = c.Content } }
                }).ToList() // Send only messages for this channel
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _logger.LogInformation("Sending message to AI for channel {channelId}...", channelId);

            var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send message to AI for channel {channelId}.", channelId);
                throw new Exception("Error communicating with AI");
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
            if (string.IsNullOrEmpty(generatedResponse))
            {
                throw new Exception("Failed to generate response");
            }

            _logger.LogInformation("Message successfully generated by the AI for channel {channelId}.", channelId);

            await _conversationService.AddMessageAsync(conversation.Id, generatedResponse, "model");

            // Return the generated response
            return generatedResponse;
        }

        public async Task<bool> SendResponseAsync(string blogPost, string channelId)
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


            if (string.IsNullOrEmpty(channelId))
            {
                throw new Exception("Channel ID is null");
            }

            var telexWebhookUrl = $"{_webhookUrl}/{channelId}";

           
            // Send the response to telex
            var telexResponse = await _httpClient.PostAsync(telexWebhookUrl, telexContent);

            if ((int)telexResponse.StatusCode != StatusCodes.Status202Accepted || !telexResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed to send response to telex");
                return false;
            }

            string responseContent = await telexResponse.Content.ReadAsStringAsync();

            _logger.LogInformation("Response successfully sent to telex");

            return true;
        }
    }
}
