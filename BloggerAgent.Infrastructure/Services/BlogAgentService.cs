using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons.Gemini;
using System.Threading.Channels;
using System.Collections.Concurrent;
using BloggerAgent.Domain.IRepositories;
using System.Data;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Infrastructure.Tooling;
using BloggerAgent.Domain.Commons.Options;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Infrastructure.Utilities;
using BloggerAgent.Infrastructure.Commons.BloggerAgent.Infrastructure.Commons;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace BloggerAgent.Infrastructure.Services
{
    public class BlogAgentService : IBlogAgentService
    {
        //private static readonly ConcurrentDictionary<string, List<ChatMessage>> conversations = new(); // Group messages by channelId

        private ILogger<BlogAgentService> _logger;
        private string _webhookUrl;
        private readonly IRequestProcessingService _requestService;
        private readonly IConversationRepository _messageRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IAIService _aiService;
        private readonly HttpHelper _httpHelper;
        private readonly ToolRouter _toolRouter;
        private readonly TaskContextAccessor _taskManager;

        public BlogAgentService(
            IOptions<TelexSetting> telexSettings, 
            ILogger<BlogAgentService> logger, 
            IRequestProcessingService requestService,
            IConversationRepository messageRepository,
            IAIService aiRepository,
            HttpHelper httpHelper,
            ToolRouter toolRouter, 
            IOrganizationRepository organizationRepository,
            TaskContextAccessor taskManager)
        {
            _webhookUrl = telexSettings.Value.WebhookUrl;
            _requestService = requestService;
            _logger = logger;
            _messageRepository = messageRepository;
            _aiService = aiRepository;
            _httpHelper = httpHelper;
            _toolRouter = toolRouter;
            _organizationRepository = organizationRepository;
            _taskManager = taskManager;
        }

        public async Task<MessageResponse> HandleUserInput(TaskRequest taskRequest)
        {
            try
            {
                //var newTaskContext = DataExtract.ExtractTaskData(taskRequest);
                var newTaskContext = _taskManager.GetTaskContext();
                await _messageRepository.AddNewMessagesAsync(newTaskContext.Message, newTaskContext, Roles.User);

                //var organizations = await _organizationRepository.GetAllAsync();

                //var organizationDetails = organizations.FirstOrDefault();

                //var blogTask = await _taskManager.ResolveAsync(newTaskContext.ContextId, newTaskContext.UserId);
                //newTaskContext.TaskId = blogTask?.Id;
                var previousMessages = newTaskContext.ChatMessages.Select(m => new ChatMessageContent()
                {
                    Role = new AuthorRole(m.Role),
                    Content = m.Content
                });

                var orgInfo = JsonSerializer.Serialize(newTaskContext.Organization);


                _logger.LogInformation("HandleUserInput: UserMessage={Message}", newTaskContext.Message);

                var aiReply = await _aiService.ChatWithTools(newTaskContext, PromptTemplate.BuildOrchestratorPrompt(orgInfo), previousMessages);
                await _messageRepository.AddNewMessagesAsync(aiReply, newTaskContext, Roles.Assistant);

                return DataExtract.ConstructResponse(taskRequest, aiReply);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HandleUserInput()");
                return DataExtract.ConstructResponse(taskRequest, "🤖 Sorry, something went wrong.");
            }
        }


        //public async Task<MessageResponse> HandleAsync(TaskRequest taskRequest)
        //{
           
        //    try
        //    {
        //        ValidationHelper.ValidateRequest(taskRequest);

        //        var blogPrompt = TaskContext.MapToTaskContext(taskRequest);


        //        // Format the blog prompt based on user input and settings
        //        var request = await _requestService.ProcessUserInputAsync(blogPrompt);

        //        // Generate the response using the formatted message
        //        var aiResponse = await _aiService.GenerateResponse(request.UserPrompt, request.SystemMessage, blogPrompt);

        //        if (string.IsNullOrEmpty(aiResponse))
        //        {
        //            throw new Exception("Failed to generate response");
        //        }

        //        return DataExtract.ConstructResponse(taskRequest, aiResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error and rethrow the exception
        //        _logger.LogError(ex, "Failed to generate blog post");
        //        throw;
        //    }
        //}                   

        public async Task<bool> SendResponseAsync(string messageContent, TaskContext taskContext)
        {             

            if (string.IsNullOrEmpty(taskContext.ContextId) || string.IsNullOrEmpty(_webhookUrl))
            {
                throw new Exception("Channel ID is null");
            }

            var apiRequest = new ApiRequest()
            {
                Url = $"{_webhookUrl}/{taskContext.ContextId}",
                Body = new
                {
                    channel_id = taskContext.ContextId,
                    org_id = taskContext.MessageId,
                    thread_id = taskContext.TaskId,
                    message = messageContent,
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


        public async Task<MessageResponse> HandleAsync(TaskRequest taskRequest)
        {
            try
            {
                ValidationHelper.ValidateRequest(taskRequest);

                var context = DataExtract.ExtractTaskData(taskRequest);
                //var request = await _requestService.ProcessUserInputAsync(context);

                string aiMessage = context.Message;
                string systemMessage = _toolRouter.BuildSystemMessage();
                string finalResponse = null;
                await _messageRepository.AddNewMessagesAsync(context.Message, context, Roles.User);
                while (true)
                {
                    var aiResponse = await _aiService.GenerateResponse(aiMessage, systemMessage, context);

                    if (!_toolRouter.ShouldRunTool(aiResponse))
                    {
                        finalResponse = aiResponse;
                        break;
                    }

                    var (toolName, aiParams) = _toolRouter.ExtractToolInfo(aiResponse);
                    var toolResult = await _toolRouter.ExecuteToolAsync(toolName, aiParams, context);

                    // Inject tool result into the next AI message
                    aiMessage = _toolRouter.FormatToolResult(toolName, toolResult);
                }
                await _messageRepository.AddNewMessagesAsync(aiMessage, context, Roles.Assistant);

                return DataExtract.ConstructResponse(taskRequest, finalResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate blog post");
                throw;
            }
        }       

    }
}
