using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task GenerateBlogAsync(GenerateBlogDto blogPrompt);
        Task SendBlogAsync(string blogPost, List<Setting> settings);
    }
}
