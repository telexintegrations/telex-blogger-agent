using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Services
{
    internal class KeywordAgent
    {
        private readonly HttpHelper _httpClient;
        private readonly string _apiKey;
        private readonly IBlogRepository _blogRepository;
        private readonly IAIService _aiService;
        private readonly TaskContextAccessor _taskContextAccessor;

        public KeywordAgent(IBlogRepository blogRepository, IAIService aIService, HttpHelper httpHelper, IConfiguration configuration, TaskContextAccessor taskContextAccessor)
        {
            _aiService = aIService;
            _blogRepository = blogRepository;
            _httpClient = httpHelper;
            _apiKey = configuration.GetValue<string>("GroqApiKey")!;
            _taskContextAccessor = taskContextAccessor;
        }

        public async Task<string> GetSeoKeywordsAsync(string topic)
        {
            var taskContext = _taskContextAccessor.GetTaskContext();
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var request = new GroqChatRequest
            {
                Messages = new List<GroqChatRequest.Message>
                {
                    new() { Role = "system", Content = PromptTemplate.GetOutlinePrompt(topic, JsonSerializer.Serialize(taskContext.Organization)) }
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
                .Select(m => new GroqChatRequest.Message
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
                    { "Authorization", $"Bearer {_apiKey}" }
                }
            };

            var response = await _httpClient.SendRequestAsync(apiRequest);

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
