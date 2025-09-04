using BloggerAgent.Application.IServices;
using BloggerAgent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MongoDB.Driver;
using System.ComponentModel;

namespace BloggerAgent.Infrastructure.ToolFunctions;

public class AgentPlugin
{
    private readonly ILogger<AgentPlugin> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AgentPlugin(
        ILogger<AgentPlugin> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

  
}

public class TopicPlugin
{

    //private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TopicPlugin(IServiceScopeFactory scopeFactory)
    {
        //_logger = logger;
        _scopeFactory = scopeFactory;
    }

    [KernelFunction("Get_Trending_Topics")]
    [Description("Get trending blog topics and SEO keywords based on user interest.")]
    public async Task<string> GetTrendingTopicsAsync(
        [Description("The area of interest like 'AI' or 'finance'.")] string interest)
    {
        using var scope = _scopeFactory.CreateScope();
        var topicAgent = scope.ServiceProvider.GetRequiredService<IResearchAgent>();

        try
        {
            return await topicAgent.GetTrendingTopicsAsync(interest);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}, Failed to generate outline for title: {interest}");
            return "⚠️ Could not retrieve trending topics at the moment.";
        }
    }

}

public class KeywordPlugin
{

    //private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public KeywordPlugin(IServiceScopeFactory scopeFactory)
    {
        //_logger = logger;
        _scopeFactory = scopeFactory;
    }

    [KernelFunction("Get_Keywords_For_Topic")]
    [Description("Get SEO keywords for a particular topic.")]
    public async Task<string> GetTrendingTopicsAsync(
        [Description("The topic to research keywords for")] string topic)
    {
        using var scope = _scopeFactory.CreateScope();
        var topicAgent = scope.ServiceProvider.GetRequiredService<IResearchAgent>();

        try
        {
            return await topicAgent.GetTrendingTopicsAsync(topic);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}, Failed to generate outline for title: {topic}");
            return "⚠️ Could not retrieve trending topics at the moment.";
        }
    }

}

public class OutlinePlugin
{

    //private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutlinePlugin(IServiceScopeFactory scopeFactory)
    {
        //_logger = logger;
        _scopeFactory = scopeFactory;
    }


    [KernelFunction("Generate_Outline")]
    [Description("Generate a structured outline for a blog post.")]
    public async Task<string> GenerateOutlineAsync(
        [Description("The blog topic/title.")] string title,
        [Description("SEO keywords related to the topic.")] string keywords)
    {
        using var scope = _scopeFactory.CreateScope();
        var outlineAgent = scope.ServiceProvider.GetRequiredService<OutlineAgent>();

        try
        {
            return await outlineAgent.GetOutlineAsync(title);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ ex}, Failed to generate outline for title: {title}");
            return "⚠️ Could not generate outline at this time.";
        }
    }


}

public class ResearchPlugin
{

    //private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ResearchPlugin(IServiceScopeFactory scopeFactory)
    {
        //_logger = logger;
        _scopeFactory = scopeFactory;
    }

    [KernelFunction]
    public Task<string> GetDateTimeAsync()
    {
        return Task.FromResult(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [KernelFunction("Get_Research_Report")]
    [Description("Gets a structured web research report on a particular topic.")]
    public async Task<string> GetResearchAsync(
        [Description("The topic to research on.")] string topic, string outline)
    {
        using var scope = _scopeFactory.CreateScope();
        var topicAgent = scope.ServiceProvider.GetRequiredService<IResearchAgent>();

        try
        {
            return await topicAgent.GetWebResearchAsync(topic, outline);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get trending topics for interest: {topic} \n {ex}");
            return "⚠️ Could not retrieve trending topics at the moment.";
        }
    }

}

public class WriterPlugin
{

    //private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public WriterPlugin(IServiceScopeFactory scopeFactory)
    {
        //_logger = logger;
        _scopeFactory = scopeFactory;
    }

    [KernelFunction("Write_Blog_Post")]
    [Description("Write a comprehensive blog post using a blog outline and research report on the blog topic.")]
    public async Task<string> GenerateBlogPostAsync(
       [Description("The research report outline to use.")] string researchOutline, string keywords)
    {
        using var scope = _scopeFactory.CreateScope();
        var writerAgent = scope.ServiceProvider.GetRequiredService<WriterAgent>();

        try
        {
            return await writerAgent.WriteBlogAsync(researchOutline, keywords);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ ex}, Failed to Write blog post.");
            return "⚠️ Blog generation failed. Please try again.";
        }

    }

}



