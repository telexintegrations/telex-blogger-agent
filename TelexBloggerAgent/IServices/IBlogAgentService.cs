using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task HandleBlogRequestAsync(GenerateBlogDto blogPrompt);
        Task RefineContentAsync(RefineBlogDto blogPrompt);
        Task SuggestTopicsAsync(GenerateBlogDto blogPrompt);
        Task GenerateBlogAsync(GenerateBlogDto blogPrompt);
        Task SendBlogAsync(string blogPost, List<Setting> settings);
    }
}
