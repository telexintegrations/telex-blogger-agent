using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IRepositories
{
    public interface IConversationRepository : IMongoRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationsByUserAsync(string userId);
        Task AddMessageAsync(string conversationId, Message message);
    }
}