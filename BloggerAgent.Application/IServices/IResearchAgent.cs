namespace BloggerAgent.Application.IServices
{
    public interface IResearchAgent
    {
        Task<string> GetTrendingTopicsAsync(string topic, string systemMessage = null);
        Task<string> GetWebResearchAsync(string topic, string systemMessage = null);
    }
}