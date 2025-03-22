using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task<string> GenerateResponse(string message, string systemMessage);
        Task<string> HandleAsync(GenerateBlogDto blogPrompt);
        Task<bool> SendBlogAsync(string blogPost, List<Setting> settings);
    }
}
