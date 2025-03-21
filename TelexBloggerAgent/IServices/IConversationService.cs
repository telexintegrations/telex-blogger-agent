using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IServices
{
    public interface IConversationService
    {
        Task AddMessageAsync(string conversationId, string messageText, string role);
        Task<Conversation?> GetConversationAsync(string conversationId);
        Task<List<Conversation>> GetUserConversationsAsync(string userId);
        Task<Conversation> StartConversationAsync(string userId, string initialMessage);
    }
}