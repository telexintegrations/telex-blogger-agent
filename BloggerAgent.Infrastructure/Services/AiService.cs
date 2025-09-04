using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.Models;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BloggerAgent.Domain.Data;
using System.Text.Json;
using BloggerAgent.Infrastructure.Tooling;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using BloggerAgent.Application.Configurations;
using Microsoft.SemanticKernel;
using BloggerAgent.Application.Dtos;
using BloggerAgent.Domain.Commons.Options;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Commons.DataEntities;

namespace BloggerAgent.Infrastructure.Services
{
    public class AIService : IAIService
    {
        private ILogger<BlogAgentService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly IConversationRepository _messageRepository;
        private readonly KernelProvider _kernelProvider;
        private readonly HttpHelper _httpHelper;


        public AIService(IOptions<TelexApiSettings> dataConfig, IOptions<TelexSetting> telexSettings, ILogger<BlogAgentService> logger, IConversationRepository messageRepository, HttpHelper httpHelper, KernelProvider kernelProvider)
        {
            _apiKey = dataConfig.Value.ApiKey;
            _baseUrl = dataConfig.Value.BaseUrl;
            _logger = logger;
            _messageRepository = messageRepository;
            _httpHelper = httpHelper;
            _kernelProvider = kernelProvider;
        }


        public async Task<string> GenerateResponse(string message, string systemMessage, TaskContext blogDto)
        {

            //bool isAdded = await AddNewMessagesAsync(message, blogDto, Roles.User);

            //if (!isAdded)
            //    return "Sorry, an error occured";

            var messages = new List<TelexChatMessage>()
            {
                new TelexChatMessage() { Role = Roles.System, Content = systemMessage }
            };

            var conversations = await _messageRepository.GetMessagesAsync(blogDto.ContextId);

            if (conversations.Count > 0 || conversations != null)
            {
                messages.AddRange(conversations);
            }

            //messages.Add(new TelexChatMessage { Role = "user", Content = message });

            var apiRequest = new ApiRequest()
            {
                Url = $"{_baseUrl}/telexai/chat",
                Body = new { messages },
                Method = HttpMethod.Post,
                Headers = new Dictionary<string, string>
                {
                    {TelexApiSettings.Header, _apiKey },
                    {"X-Model", "google/gemini-2.5-flash-preview-05-20" }
                }
            };

            _logger.LogInformation("Sending message to Telex AI");

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = TelexApiResponse<TelexChatMessage>.ExtractResponse(responseString);

                return $"An error occurred while communicating with Telex AI: {error.Message}";
            }

            _logger.LogInformation("Message successfully generated from the Telex AI");

            var generatedData = TelexApiResponse<TelexChatResponse>.ExtractResponse(responseString);

            string generatedResponse = generatedData.Data.Messages.Content;

            //await AddNewMessagesAsync(message, blogDto, Roles.Assistant);

            messages.Add(new TelexChatMessage() { Role = Roles.Assistant, Content = systemMessage });

            return generatedResponse;
        }


        public async Task<string> ChatWithTools(TaskContext taskRequest, string systemPrompt, IEnumerable<ChatMessageContent> messages)
        {
            try
            {
                var kernel = _kernelProvider.Kernel;
                var chatService = _kernelProvider.ChatCompletionService;

                // Save user message
                //await _messageRepository.AddNewMessagesAsync(taskRequest.Message, taskRequest, Roles.User);

                //var previousMessages = await _messageRepository.GetMessagesAsync(taskRequest.ContextId);

                var history = new ChatHistory();

                // Add system message to guide the assistant
                history.AddSystemMessage(systemPrompt);

                // Add prior conversation messages
                //history.AddRange(previousMessages.Select(m => new ChatMessageContent()
                //{
                //    Role = new AuthorRole(m.Role),
                //    Content = m.Content
                //}));
                history.AddRange(messages);
                history.AddUserMessage(taskRequest.Message);

                // Enable Function Calling
                var executionSettings = new GeminiPromptExecutionSettings
                {
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,

                    Temperature = 0.7,               // Controls randomness; lower is more deterministic
                    TopP = 0.9,                      // Nucleus sampling; focuses on top cumulative probability tokens
                    TopK = 40,                       // Limits sampling to top-k probable tokens
                };

                var result = await chatService.GetChatMessageContentAsync(
                    history,
                    executionSettings: executionSettings,
                    kernel: kernel
                );

                // Save AI reply (optional)
                //await _messageRepository.AddNewMessagesAsync(result.Content, taskRequest, Roles.Assistant);

                return result.Content ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An Error occured during AI Chat with message: {ex.Message}");
                return "Sorry, something went wrong.";
            }
        }
        
        public async Task<string> GenerateAsync(string systemPrompt, TaskContext taskRequest = null, IEnumerable<ChatMessageContent> messages = null, string userMessage = null)
        {
            try
            {
                var kernel = _kernelProvider.Kernel;
                var chatService = _kernelProvider.ChatCompletionService;

                // Save user message
              
                var history = new ChatHistory();

                // Add system message to guide the assistant
                history.AddSystemMessage(systemPrompt);

                if (messages != null)
                {
                    history.AddRange(messages);
                }

                if (userMessage != null) 
                {
                    history.AddUserMessage(userMessage);
                }
                var result = await chatService.GetChatMessageContentAsync(history);

                
                return result.Content ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An Error occured during AI Chat with message: {ex.Message}");
                return "Sorry, something went wrong.";
            }
        }

        public async Task<string> GenerateResponseAsync(string systemPrompt, TaskContext taskRequest)
        {
            try
            {
                var kernel = _kernelProvider.Kernel;
                var chatService = _kernelProvider.ChatCompletionService;

                var history = new ChatHistory();
                history.AddSystemMessage(systemPrompt);
                history.AddUserMessage(taskRequest.Message); // Optional based on flow

                // Step 1: Ask SK what to do next
                var nextActionResult = await kernel.Plugins
                    .GetFunction("Blog", "GetNextAction")
                    .InvokeAsync(kernel, new KernelArguments
                    {
                        ["phase"] = taskRequest.BlogTask.CurrentPhase.ToString()
                    });

                var nextTool = nextActionResult.GetValue<string>();
                if (string.IsNullOrWhiteSpace(nextTool))
                {
                    return "Agent couldn't determine the next action.";
                }

                // Step 2: Call tool dynamically
                var function = kernel.Plugins.GetFunction("BlogPlugin", nextTool);
                var toolResponse = await function.InvokeAsync(kernel, new KernelArguments
                {
                    ["interest"] = taskRequest.Message // or other contextual input
                });

                var assistantReply = toolResponse.GetValue<string>();

                // Step 3: Respond as assistant
                history.AddAssistantMessage(assistantReply);

                var finalReply = await chatService.GetChatMessageContentAsync(history);

                return finalReply.Content ?? assistantReply ?? "No content generated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateAsync()");
                return "Sorry, something went wrong.";
            }
        }


        public static string BuildSystemMessage()
        {
            return $$$"""
                You are a professional blogging assistant whose responsibilities include:
                1. Recommending topics, suggesting keywords, and brainstorming ideas.
                2. Presenting a clear outline—title, headings, and bullet-pointed structure—for approval.
                3. After outline approval, generating the full blog in the order: title, introduction, body sections, and conclusion using the organization contextual data.
                
                Always retrieve user organization before making a save inorder for to determine whether you are to save or update.

                Don't ask users for more information until you have retrieved their organization context data. If no information is recorded, ask the user for their organization information. But if an information is provided in the request, go ahead and use the information provided. 
                
                When provided with an organizational information, get confirmation before proceeding to save it. If the information provided is not explicit enough, you can try to deduce most of these other fields based on the information provided and confirm the information with the user before proceeding to save it.

                Don't share your internal thought process with the user

                Always keep your tone professional and focused on high-quality, actionable blogging guidance but maintain a friendly demeanour.
                """;
        }
    }
}
