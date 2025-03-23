using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogPostIntervalService
    {
        void ScheduleBlogPostGeneration(string option, GenerateBlogDto blogPrompt);
    }
}
