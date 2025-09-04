using BloggerAgent.Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.ToolFunctions
{
    public class AgentPlugin
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AgentPlugin(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [KernelFunction("Get_Date_Time")]
        public Task<string> GetDateTime()
        {
            return Task.FromResult(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [KernelFunction("search_the_web_for_trending_topics")]
        [Description("Finds trending topics and keywords based on an area of interest")]
        [return: Description("The trending topics and keywords based on the provided area of interest")]
        public async Task<string> GetTrendingTopics([Description("Prompt for the research")] string prompt)
        {

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IResearchAgent>();

            var trends = await repo.GetTrendingTopicsAsync(prompt);
            return trends;
        }
    }
}
