using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.Gemini;
using BloggerAgent.Domain.Models;
using System.Reflection.Metadata;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IConversationRepository : IMongoRepository<Message>
    {
        Task<Document<Message>> GetConversationsByUserAsync(string userId);
        Task<List<ChatMessage>> GetMessagesAsync(string contextId);
    }
}