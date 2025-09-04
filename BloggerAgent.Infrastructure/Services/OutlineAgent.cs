using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Commons.DataEntities;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Services
{
    public class OutlineAgent
    {
        private readonly HttpHelper _httpClient;
        private readonly string _apiKey;
        private readonly IBlogRepository _blogRepository;
        private readonly IAIService _aiService;
        private readonly TaskContextAccessor _taskContextAccessor;

        public OutlineAgent(IBlogRepository blogRepository, IAIService aIService, HttpHelper httpHelper, IConfiguration configuration, TaskContextAccessor taskContextAccessor)
        {
            _aiService = aIService;
            _blogRepository = blogRepository;
            _httpClient = httpHelper;
            _apiKey = configuration.GetValue<string>("GroqApiKey")!;
            _taskContextAccessor = taskContextAccessor;
        }

               
        public async Task<string> GetOutlineAsync(string topic)
        {
            var taskContext = _taskContextAccessor.GetTaskContext();
            if (taskContext == null)
            {
                return "Task context is not available.";
            }

            string systemMessage = PromptTemplate.GetOutlinePrompt(topic, JsonSerializer.Serialize(taskContext.Organization));
            return await _aiService.GenerateReponse(systemMessage, taskContext);
            
        }            

    }
}
