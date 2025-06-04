using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons.Gemini;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainService;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BloggerAgent.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private ILogger<BlogAgentService> _logger;
        private readonly string _apiKey;
        private readonly string _geminiUrl;
        private readonly IConversationRepository _messageRepository;
        private readonly HttpHelper _httpHelper;


        public AiService(
            IOptions<GeminiSetting> geminiSettings,
            IOptions<TelexSetting> telexSettings,
            ILogger<BlogAgentService> logger,
            IConversationRepository messageRepository,
            HttpHelper httpHelper)
        {
            _apiKey = geminiSettings.Value.ApiKey;
            _geminiUrl = geminiSettings.Value.GeminiUrl;
            _logger = logger;
            _messageRepository = messageRepository;
            _httpHelper = httpHelper;
        }
        public async Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogTask blogDto)
        {
            //var conversation = await _conversationService.GetUserConversationsAsync(channelId);
            List<ChatMessage> conversation = await _messageRepository.GetMessagesAsync(blogDto.ContextId);

            conversation.Add(new ChatMessage
            {
                Role = "user",
                Parts = { new Part { Text = message } }
            });        

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
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (errorResponse.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetProperty("message").GetString();
                    _logger.LogError($"AI API Error: {errorMessage}");
                    return $"An error occurred while communicating with the AI: {errorMessage}";
                }

                _logger.LogError($"Failed to communicate with the AI agent: {errorResponse}");
                return $"An error occured while communicating with the AI. Please try again later";

            }
            _logger.LogInformation("Message successfully generated from the AI");

            string? generatedResponse = DataExtract.ExtractAiResponseData(responseString);           


            bool isAdded = await _messageRepository.CreateAsync(new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = message,
                TaskId = blogDto.TaskId,
                ContextId = blogDto.ContextId,
                Role = "user"
            });

            if (!isAdded)
                _logger.LogInformation("Failed to add user message to database");

            bool isSuccess = await _messageRepository.CreateAsync(new Message()
            {
                Id = Guid.NewGuid().ToString(),
                Content = generatedResponse,
                TaskId = blogDto.TaskId,
                ContextId = blogDto.ContextId,
                Role = "model"
            });

            if (!isSuccess)
                _logger.LogInformation("Failed to add AI response to database");

            return generatedResponse;
        }
    }
}
