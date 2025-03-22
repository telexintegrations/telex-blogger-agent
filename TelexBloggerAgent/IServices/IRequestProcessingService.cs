using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Helpers;

namespace TelexBloggerAgent.IServices
{
    public interface IRequestProcessingService
    {
        Task<Request> ProcessUserInputAsync(GenerateBlogDto blogDto);
    }
}