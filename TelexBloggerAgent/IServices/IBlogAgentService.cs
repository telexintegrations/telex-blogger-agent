using TelexBloggerAgent.Dtos;

namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task<string?> GenerateBlogAsync(GenerateBlogDto blogPrompt);
    }
}
