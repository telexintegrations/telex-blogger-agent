using BloggerAgent.Application.Dtos;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IRequestProcessingService
    {
        Task<Request> ProcessUserInputAsync(GenerateBlogTask blogDto);
        string GetBlogIntervalOption(GenerateBlogTask blogDto);
        Task<Request> ProcessRefinementRequestAsync(RefineBlogDto blogDto);
    }
}