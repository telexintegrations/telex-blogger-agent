using BloggerAgent.Application.Dtos.A2ATaskDtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogAgentService
    {
        //Task<AgentMessageResponse> HandleMessageAsync(A2aTaskRequest taskRequest);
        Task HandleUserInput(A2aTaskRequest taskRequest);
        //Task<string> HandleAsync(A2aTaskRequest taskRequest);
        Task<bool> SendResponseAsync(AgentTaskResponse taskResponse, TaskContext taskContext);
    }
}
