using BloggerAgent.Application.Dtos;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogAgentService
    {
        Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogDto blogDto);
        Task<string> HandleAsync(GenerateBlogDto blogPrompt);
        Task<bool> SendResponseAsync(string blogPost, GenerateBlogDto blogDto);
    }
}
