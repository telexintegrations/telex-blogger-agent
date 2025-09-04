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
using BloggerAgent.Infrastructure.Utilities;
using System.Net.Http;
using System.Text;

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


        public async Task<string> GenerateReponse(string systemMessage, TaskContext context)
        {
            TaskContext taskContext = context;
           
            var url = "https://api.telex.im/api/v1/telexai/chat";

            var request = new
            {
                Model = "google/gemini-2.0-flash-001",
                Messages = new List<TelexChatMessage>
                {
                    new() { Role = "system", Content = systemMessage }
                }
            };

            // ✅ Add chat history from task context
            var chatHistory = taskContext.ChatMessages;

            chatHistory.Add(new TelexChatMessage()
            {
                Role = "user",
                Content = taskContext.Message
            });

            var historyMessages = chatHistory
                .Select(m => new TelexChatMessage
                {
                    Role = m.Role,  // Ensure these are "user" or "assistant"
                    Content = m.Content
                });

            request.Messages.AddRange(historyMessages); // ✅ Add the actual history

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ApiRequest apiRequest = new ApiRequest
            {
                Url = url,
                Body = request,
                Method = HttpMethod.Post,
                Headers = new Dictionary<string, string>
                {
                    { "X-AGENT-API-KEY", taskContext.AuthToken }
                }
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Telex API Call failed: {response.StatusCode}");
            }

            string responseJson = await response.Content.ReadAsStringAsync();

            JsonElement docRootElement = JsonDocument.Parse(responseJson).RootElement;

            if (!docRootElement.TryGetProperty("data", out JsonElement data))
            {
                Console.WriteLine($"Error: No 'data' property found in response: {responseJson}");
                throw new Exception("No data found in Telex API response");
            }

            GroqChatResponse? result = JsonSerializer.Deserialize<GroqChatResponse>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "Couldn't generate any response";
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
                if (messages != null && messages.Any())
                {
                    history.AddRange(messages);
                }

                history.AddUserMessage(taskRequest.Message);

                // Enable Function Calling
                var executionSettings = new GeminiPromptExecutionSettings
                {
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,

                    Temperature = 0.8,               // Controls randomness; lower is more deterministic
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

                if (messages != null && messages.Any())
                {
                    history.AddRange(messages);
                }

                if (!string.IsNullOrEmpty(userMessage) || !string.IsNullOrEmpty(taskRequest?.Message))
                {
                    history.AddUserMessage(userMessage ?? taskRequest.Message);
                }

                // Enable Function Calling
                var executionSettings = new GeminiPromptExecutionSettings
                {                   
                    Temperature = 0.8,               // Controls randomness; lower is more deterministic
                    TopP = 0.9,                      // Nucleus sampling; focuses on top cumulative probability tokens
                    TopK = 40,                       // Limits sampling to top-k probable tokens
                };
                var result = await chatService.GetChatMessageContentAsync(history, executionSettings, kernel);
                
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


        public async Task<string> GenerateWebContentAsync(string systemMessage, TaskContext taskContext)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var request = new GroqChatRequest
            {
                Messages = new List<GroqChatRequest.Message>
                {
                    new() { Role = "system", Content = systemMessage }
                }
            };

            // ✅ Add chat history from task context

            var chatHistory = taskContext?.ChatMessages;

            if (chatHistory != null)
            {
                chatHistory.Add(new TelexChatMessage()
                {
                    Role = "user",
                    Content = taskContext.Message
                });

                var historyMessages = chatHistory
                    .Select(m => new GroqChatRequest.Message
                    {
                        Role = m.Role,  // Ensure these are "user" or "assistant"
                        Content = m.Content
                    });

                request.Messages.AddRange(historyMessages); // ✅ Add the actual history

            }

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ApiRequest apiRequest = new ApiRequest
            {
                Url = url,
                Body = request,
                Method = HttpMethod.Post,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {_apiKey}" }
                }
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Groq API failed: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GroqChatResponse>(responseJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "Couldn't generate any response";
        }
    }
}
