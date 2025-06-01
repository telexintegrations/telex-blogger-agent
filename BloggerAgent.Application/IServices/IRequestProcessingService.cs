using BloggerAgent.Application.Dtos;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IRequestProcessingService
    {
        Task<Request> ProcessUserInputAsync(GenerateBlogDto blogDto);
        string GetBlogIntervalOption(GenerateBlogDto blogDto);
        Task<Request> ProcessRefinementRequestAsync(RefineBlogDto blogDto);
    }
}