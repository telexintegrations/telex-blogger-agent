using BloggerAgent.Application.Helpers;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogPostIntervalService
    {
        void ScheduleBlogPostGeneration(string option, GenerateBlogTask blogPrompt);
    }
}
