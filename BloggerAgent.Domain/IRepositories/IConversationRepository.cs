using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Commons.Gemini;
using BloggerAgent.Domain.Models;
using MongoDB.Driver.Authentication;
using System.Reflection.Metadata;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IConversationRepository : IMongoRepository<Message>
    {
        Task<Message> GetConversationsByUserAsync(string userId);
        Task<List<TelexChatMessage>> GetMessagesAsync(string contextId);

        Task<bool> AddNewMessagesAsync(string message, TaskContext blogDto, string role);
    }
}