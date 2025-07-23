namespace BloggerAgent.Application.IServices
{
    public interface IResearchAgent
    {
        Task<string> GetTrendingTopicsAsync(string topic);
    }
}