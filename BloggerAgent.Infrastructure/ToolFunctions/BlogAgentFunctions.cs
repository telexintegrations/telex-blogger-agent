using BloggerAgent.Application.IServices;
using BloggerAgent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace BloggerAgent.Infrastructure.ToolFunctions;

public class BlogAgentFunctions
{
    private readonly ILogger<BlogAgentFunctions> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BlogAgentFunctions(
        ILogger<BlogAgentFunctions> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    [KernelFunction("GetTrendingTopics")]
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
            _logger.LogError(ex, "Failed to get trending topics for interest: {Interest}", interest);
            return "⚠️ Could not retrieve trending topics at the moment.";
        }
    }
    
    [KernelFunction("GetWebResearch")]
    [Description("Gets a structured web research report on a particular topic.")]
    public async Task<string> GetResearchAsync(
        [Description("The topic to research on.")] string topic)
    {
        using var scope = _scopeFactory.CreateScope();
        var topicAgent = scope.ServiceProvider.GetRequiredService<IResearchAgent>();

        try
        {
            return await topicAgent.GetWebResearchAsync(topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get trending topics for interest: {Interest}", topic);
            return "⚠️ Could not retrieve trending topics at the moment.";
        }
    }

    [KernelFunction("GenerateOutline")]
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
            _logger.LogError(ex, "Failed to generate outline for title: {Title}", title);
            return "⚠️ Could not generate outline at this time.";
        }
    }

    [KernelFunction("GenerateBlogPost")]
    [Description("Generate a compelling full blog post from outline.")]
    public async Task<string> GenerateBlogPostAsync(
        [Description("The blog outline to use.")] string outline, string keywords, string sources)
    {
        using var scope = _scopeFactory.CreateScope();
        var writerAgent = scope.ServiceProvider.GetRequiredService<WriterAgent>();

        try
        {
            return await writerAgent.WriteBlogAsync(outline, keywords, sources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate blog post.");
            return "⚠️ Blog generation failed. Please try again.";
        }

    }
        //[KernelFunction(name: "GetNextAction")]
        //public string GetNextAction([Description("The phase to execute")] string phase)
        //{
        //    //var currentPhase = 
        //    return phase switch
        //    {
        //        "Initialized" => "GetTrendingTopics",
        //        "KeywordDiscovery" => "GenerateOutline",
        //        "OutlineGenerated" => "GenerateIntro",
        //        _ => "GetTrendingTopics"
        //    };
        //}
}

