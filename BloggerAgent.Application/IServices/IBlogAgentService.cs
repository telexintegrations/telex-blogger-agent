using BloggerAgent.Application.Dtos;
using BloggerAgent.Application.Helpers;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogAgentService
    {
        Task<string> GenerateResponse(string message, string systemMessage, GenerateBlogTask blogDto);
        Task<MessageResponse> HandleAsync(TaskRequest taskRequest);
        Task<bool> SendResponseAsync(string blogPost, GenerateBlogTask blogDto);
    }
}
