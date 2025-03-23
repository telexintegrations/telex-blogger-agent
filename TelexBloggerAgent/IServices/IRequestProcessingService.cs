using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Helpers;

namespace TelexBloggerAgent.IServices
{
    public interface IRequestProcessingService
    {
        Task<Request> ProcessUserInputAsync(GenerateBlogDto blogDto);
        string GetBlogIntervalOption(GenerateBlogDto blogDto);
        Task<Request> ProcessRefinementRequestAsync(RefineBlogDto blogDto);
    }
}