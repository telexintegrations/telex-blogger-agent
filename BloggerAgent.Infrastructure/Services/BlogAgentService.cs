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
        private readonly IRequestProcessingService _requestService;
        private readonly IConversationRepository _messageRepository;
        private readonly IAiService _aiService;
        private readonly HttpHelper _httpHelper;

        public BlogAgentService(
            IOptions<TelexSetting> telexSettings, 
            ILogger<BlogAgentService> logger, 
            IRequestProcessingService requestService,
            IConversationRepository messageRepository,
            IAiService aiRepository,
            HttpHelper httpHelper)
        {
            _webhookUrl = telexSettings.Value.WebhookUrl;
            _requestService = requestService;
            _logger = logger;
            _messageRepository = messageRepository;
            _aiService = aiRepository;
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
                var aiResponse = await _aiService.GenerateResponse(request.UserPrompt, request.SystemMessage, blogPrompt);

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
                    reply = false, 
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
