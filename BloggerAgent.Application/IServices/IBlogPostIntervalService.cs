using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.IServices
{
    public interface IBlogPostIntervalService
    {
        void ScheduleBlogPostGeneration(string option, TaskContext blogPrompt);
    }
}
