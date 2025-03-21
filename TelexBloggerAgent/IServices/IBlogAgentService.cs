using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task<string> GenerateResponse(string message);
        Task HandleAsync(GenerateBlogDto blogPrompt);
        Task<bool> SendBlogAsync(string blogPost, List<Setting> settings);
    }
}
