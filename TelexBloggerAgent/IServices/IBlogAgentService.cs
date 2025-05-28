using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task<string> GenerateResponse(string message, string systemMessage, string channelId);
        Task<string> HandleAsync(GenerateBlogDto blogPrompt);
        Task<bool> SendResponseAsync(string blogPost, string channelId);
    }
}
