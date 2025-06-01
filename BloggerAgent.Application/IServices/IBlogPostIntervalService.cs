using BloggerAgent.Application.Dtos;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogPostIntervalService
    {
        void ScheduleBlogPostGeneration(string option, GenerateBlogDto blogPrompt);
    }
}
