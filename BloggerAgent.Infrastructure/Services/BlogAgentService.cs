using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons.Gemini;
using System.Threading.Channels;
using System.Collections.Concurrent;
using BloggerAgent.Domain.Models;
using BloggerAgent.Domain.IRepositories;
using System.Data;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;
using BloggerAgent.Application.Dtos;
using Microsoft.AspNetCore.Http;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainService;

namespace BloggerAgent.Infrastructure.Services
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
        private readonly IMongoRepository<Message> _messageRepository;
        private readonly HttpHelper _httpHelper;


        public BlogAgentService(
            IHttpClientFactory httpClientFactory, 
            IOptions<GeminiSetting> geminiSettings, 
            IOptions<TelexSetting> telexSettings, 
            ILogger<BlogAgentService> logger, 
            IRequestProcessingService requestService,
            IConversationService conversationService,
            IMongoRepository<Message> messageRepository,
            HttpHelper httpHelper)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _webhookUrl = telexSettings.Value.WebhookUrl;
            _requestService = requestService;
            _logger = logger;
            _conversationService = conversationService;
            _messageRepository = messageRepository;
            _httpHelper = httpHelper;
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
                var aiResponse = await GenerateResponse(request.UserPrompt, request.SystemMessage, blogPrompt);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    throw new Exception("Failed to generate response");
                }

                // Append the identifier to the generated agent response
                var signedResponse = $"{aiResponse}\n\n{identifier}";

                // Send the generated blog post to Telex
                //var suceeded = await SendResponseAsync(signedResponse, blogPrompt);

                
                //if (!suceeded)
                //{
                //    throw new Exception("Failed to send blog post to Telex");
                //}

                return signedResponse;

            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Failed to generate blog post");
                throw;
            }
        }

        public async Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogDto blogDto)
        {
            //var conversation = await _conversationService.GetUserConversationsAsync(channelId);

            bool isAdded = await _messageRepository.CreateAsync(new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = blogDto.Message,
                ConversationId = blogDto.ThreadId,
                SourceId = blogDto.ChannelId,
                Role = "user"
            });

            if (!isAdded)
                throw new Exception("Failed to add messege to database");

            List<ChatMessage> conversation = await GetMessages(blogDto);

            if (conversation == null || conversation.Count == 0)
            {
                throw new Exception("Failed to retrieve messages");
            }

            var apiRequest = new ApiRequest()
            {
                Url = $"{_geminiUrl}?key={_apiKey}",
                Body = new GeminiRequest
                {
                    SystemInstruction = new SystemMessage
                    {
                        Parts = { Text = systemMessage }
                    },
                    //Contents = conversations[channelId] // Send only messages for this channel
                    Contents = conversation
                },
                Method = HttpMethod.Post,
            };

            _logger.LogInformation("Sending message to AI for channel {channelId}...", blogDto.ChannelId);

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send message to AI for channel {channelId}.", blogDto.ChannelId);
                throw new Exception("Error communicating with AI");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            string? generatedResponse = ExtractGeneratedResponse(responseString);

            if (string.IsNullOrEmpty(generatedResponse))
            {
                throw new Exception("Failed to generate response");
            }

            _logger.LogInformation("Message successfully generated by the AI for channel {channelId}.", blogDto.ChannelId);

            bool isSuccess = await _messageRepository.CreateAsync(new Message()
            {
                Id = Guid.NewGuid().ToString(),
                Content = generatedResponse,
                ConversationId = blogDto.ThreadId,
                SourceId = blogDto.ChannelId,
                Role = "model"
            });

            if (!isSuccess)
                throw new Exception("Failed to add AI response to database");

            return generatedResponse;
        }

        private async Task<List<ChatMessage>> GetMessages(GenerateBlogDto blogDto)
        {
            var conversations = await _messageRepository.FilterAsync(new { tag = CollectionType.Message });
            if (conversations == null || conversations.Count == 0)
            {
                throw new Exception("Failed to retrieve messages");
            }

            var conversation = conversations
                .Where(c => c.Data.SourceId == blogDto.ChannelId)
                .Select(c => new ChatMessage()
                {
                    Role = c.Data.Role,
                    Parts = { new Part { Text = c.Data.Content } }
                }).ToList();
            return conversation;
        }

        private string? ExtractGeneratedResponse(string responseString)
        {
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
            return generatedResponse;
        }

        public async Task<bool> SendResponseAsync(string blogPost, GenerateBlogDto blogDto)
        {

            /*Define the payload for the telex channel

           var payload = new
           {
               event_name = "Blog AI",
               message = blogPost,
               status = "success",
               username = "Blogger Agent"
           };*/

            if (string.IsNullOrEmpty(blogDto.ChannelId) || string.IsNullOrEmpty(_webhookUrl))
            {
                throw new Exception("Channel ID is null");
            }

            var apiRequest = new ApiRequest()
            {
                Url = $"{_webhookUrl}/{blogDto.ChannelId}",
                Body = new
                {
                    channel_id = blogDto.ChannelId,
                    org_id = blogDto.OrganizationId,
                    thread_id = blogDto.ThreadId,
                    message = blogPost,
                    reply = false, // set to `true` if replying to the message in a thread
                    username = "Blogger Agent"
                },
                Method = HttpMethod.Post,
            };

            var telexResponse = await _httpHelper.SendRequestAsync(apiRequest);
           
            if ((int)telexResponse.StatusCode != StatusCodes.Status202Accepted || !telexResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failed to send response to telex");
                return false;
            }

            string responseContent = await telexResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response successfully sent to telex: {responseContent}");

            return true;
        }
    }
}
