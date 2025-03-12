namespace TelexBloggerAgent.IServices
{
    public interface IBlogAgentService
    {
        Task<string?> GenerateBlogAsync(string blogPrompt);
    }
}
