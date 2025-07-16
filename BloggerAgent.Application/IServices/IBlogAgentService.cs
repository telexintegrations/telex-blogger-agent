using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogAgentService
    {
        Task<MessageResponse> HandleAsync(TaskRequest taskRequest);
        Task<MessageResponse> HandleUserInput(TaskRequest taskRequest);
        Task<bool> SendResponseAsync(string blogPost, TaskContext blogDto);
    }
}
