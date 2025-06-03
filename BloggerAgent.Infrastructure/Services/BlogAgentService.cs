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
using BloggerAgent.Application.Helpers;

namespace BloggerAgent.Infrastructure.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        //private static readonly ConcurrentDictionary<string, List<ChatMessage>> conversations = new(); // Group messages by channelId

        private ILogger<BlogAgentService> _logger;
        private string _webhookUrl;
        private readonly string _apiKey;
        private readonly string _geminiUrl;
        private readonly IRequestProcessingService _requestService;
        private readonly IConversationRepository _messageRepository;
        private readonly HttpHelper _httpHelper;


        public BlogAgentService(
            IOptions<GeminiSetting> geminiSettings, 
            IOptions<TelexSetting> telexSettings, 
            ILogger<BlogAgentService> logger, 
            IRequestProcessingService requestService,
            IConversationRepository messageRepository,
            HttpHelper httpHelper)
        {
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _webhookUrl = telexSettings.Value.WebhookUrl;
            _requestService = requestService;
            _logger = logger;
            _messageRepository = messageRepository;
            _httpHelper = httpHelper;
        }       

        public async Task<MessageResponse> HandleAsync(TaskRequest taskRequest)
        {
           
            try
            {
                ValidationHelper.ValidateRequest(taskRequest);

                var blogPrompt = GenerateBlogTask.MapToGenerateBlogDto(taskRequest);


                // Format the blog prompt based on user input and settings
                var request = await _requestService.ProcessUserInputAsync(blogPrompt);

                // Generate the response using the formatted message
                var aiResponse = await GenerateResponse(request.UserPrompt, request.SystemMessage, blogPrompt);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    throw new Exception("Failed to generate response");
                }

                return DataExtract.ConstructResponse(taskRequest, aiResponse);

            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Failed to generate blog post");
                throw;
            }
        }       


        public async Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogTask blogDto)
        {
            //var conversation = await _conversationService.GetUserConversationsAsync(channelId);

            bool isAdded = await _messageRepository.CreateAsync(new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = blogDto.Message,
                TaskId = blogDto.TaskId,
                ContextId = blogDto.ContextId,
                Role = "user"
            });

            if (!isAdded)
                throw new Exception("Failed to add messege to database");

            List<ChatMessage> conversation = await _messageRepository.GetMessagesAsync(blogDto.ContextId);

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

            _logger.LogInformation("Sending message to AI");

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to communicate with the AI agent");
                throw new Exception("Error communicating with AI agent");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            string? generatedResponse = DataExtract.ExtractAiResponseData(responseString);

            if (string.IsNullOrEmpty(generatedResponse))
            {
                throw new Exception("Failed to generate response from the AI");
            }

            _logger.LogInformation("Message successfully generated from the AI");


            bool isSuccess = await _messageRepository.CreateAsync(new Message()
            {
                Id = Guid.NewGuid().ToString(),
                Content = generatedResponse,
                TaskId = blogDto.TaskId,
                ContextId = blogDto.ContextId,
                Role = "model"
            });

            if (!isSuccess)
                throw new Exception("Failed to add AI response to database");

            return generatedResponse;
        }                   

        public async Task<bool> SendResponseAsync(string blogPost, GenerateBlogTask blogDto)
        {             

            if (string.IsNullOrEmpty(blogDto.ContextId) || string.IsNullOrEmpty(_webhookUrl))
            {
                throw new Exception("Channel ID is null");
            }

            var apiRequest = new ApiRequest()
            {
                Url = $"{_webhookUrl}/{blogDto.ContextId}",
                Body = new
                {
                    channel_id = blogDto.ContextId,
                    org_id = blogDto.MessageId,
                    thread_id = blogDto.TaskId,
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
