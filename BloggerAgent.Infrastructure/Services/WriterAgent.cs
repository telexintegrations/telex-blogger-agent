using BloggerAgent.Application.Configurations;
using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.Commons.Options;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Services
{
    public class WriterAgent
    {
        private readonly HttpHelper _httpClient;
        private readonly string _apiKey;
        private readonly IBlogRepository _blogRepository;
        private readonly IAIService _aiService;
        private readonly TaskContextAccessor _taskContextAccessor;

        public WriterAgent(IBlogRepository blogRepository, IAIService aIService, HttpHelper httpHelper, IConfiguration configuration, TaskContextAccessor taskContextAccessor)
        {
            _aiService = aIService;
            _blogRepository = blogRepository;
            _httpClient = httpHelper;
            _apiKey = configuration.GetValue<string>("GroqApiKey");
            _taskContextAccessor = taskContextAccessor;
        }

        public async Task<string> WriteBlogAsync(string outline, string keywords)
        {
            var taskContext = _taskContextAccessor.GetTaskContext();
            // ✅ Add chat history from task context
            var chatHistory = taskContext.ChatMessages;

            var previousMessages = chatHistory.Select(m => new ChatMessageContent()
            {
                Role = new AuthorRole(m.Role),
                Content = m.Content
            });

            var orgInfo = JsonSerializer.Serialize(taskContext.Organization);

            var response = await _aiService.GenerateAsync(PromptTemplate.GenerateBlogPrompt(orgInfo, outline, keywords), messages: previousMessages, userMessage: taskContext.Message);
            return response;
        }


    }
}
