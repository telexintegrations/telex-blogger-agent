using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IConversationRepository : IMongoRepository<Conversation>
    {
        Task<Conversation> GetConversationsByUserAsync(string userId);
        Task AddMessageAsync(string conversationId, Message message);
    }
}