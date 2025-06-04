using BloggerAgent.Application.Dtos;
using BloggerAgent.Application.Helpers;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogAgentService
    {
        Task<MessageResponse> HandleAsync(TaskRequest taskRequest);
        Task<bool> SendResponseAsync(string blogPost, GenerateBlogTask blogDto);
    }
}
