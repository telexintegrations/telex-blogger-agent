using BloggerAgent.Application.Dtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IRequestProcessingService
    {
        Task<Request> ProcessUserInputAsync(TaskContext blogDto);
        string GetBlogIntervalOption(TaskContext blogDto);
        Task<Request> ProcessRefinementRequestAsync(RefineBlogDto blogDto);
    }
}