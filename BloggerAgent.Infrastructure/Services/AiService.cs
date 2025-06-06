using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainService;
using BloggerAgent.Domain.Models;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BloggerAgent.Domain.Data;

namespace BloggerAgent.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private ILogger<BlogAgentService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly IConversationRepository _messageRepository;
        private readonly HttpHelper _httpHelper;


        public AiService(
            IOptions<TelexApiSettings> dataConfig,
            IOptions<TelexSetting> telexSettings,
            ILogger<BlogAgentService> logger,
            IConversationRepository messageRepository,
            HttpHelper httpHelper)
        {
            _apiKey = dataConfig.Value.ApiKey;
            _baseUrl = dataConfig.Value.BaseUrl;
            _logger = logger;
            _messageRepository = messageRepository;
            _httpHelper = httpHelper;
        }
        public async Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogTask blogDto)
        {
            var messages = new List<TelexChatMessage>()
            {
                new TelexChatMessage() { Role = "system", Content = systemMessage }
            };

            var conversations = await _messageRepository.GetMessagesAsync(blogDto.ContextId);

            if (conversations.Count > 0 || conversations != null)
            {
                messages.AddRange(conversations);
            }

            messages.Add(new TelexChatMessage { Role = "user", Content = message });

            var apiRequest = new ApiRequest()
            {
                Url = $"{_baseUrl}/telexai/chat",
                Body = new
                {
                    messages
                },
                Method = HttpMethod.Post,
                Headers = new Dictionary<string, string>
                {
                    {TelexApiSettings.Header, _apiKey },
                    {"X-Model", "google/gemini-2.5-flash-preview-05-20" }
                }
            };

            _logger.LogInformation("Sending message to AI");

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = TelexApiResponse<TelexChatMessage>.ExtractResponse(responseString);

                return $"An error occurred while communicating with the AI: {error.Message}";
            }

            _logger.LogInformation("Message successfully generated from the AI");

            var generatedData = TelexApiResponse<TelexChatResponse>.ExtractResponse(responseString);

            string generatedResponse = generatedData.Data.Messages.Content;

            await AddNewMessagesAsync(message, blogDto, generatedResponse);

            return generatedResponse;
        }

        private async Task AddNewMessagesAsync(string message, GenerateBlogTask blogDto, string generatedResponse)
        {
            var newMessages = new List<Message>()
            {
                new Message { Id = Guid.NewGuid().ToString(), Content = message, TaskId = blogDto.TaskId, ContextId = blogDto.ContextId, Role = "user" },
                new Message() { Id = Guid.NewGuid().ToString(), Content = generatedResponse, TaskId = blogDto.TaskId, ContextId = blogDto.ContextId, Role = "assistant" }
            };

            foreach (Message newMessage in newMessages)
            {
                bool isAdded = await _messageRepository.CreateAsync(newMessage);

                if (!isAdded)
                    _logger.LogInformation($"Failed to add {newMessage.Role} message to database");
            }
        }
    }
}
