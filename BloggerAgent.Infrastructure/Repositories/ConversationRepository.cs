using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BloggerAgent.Domain.Data;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using BloggerAgent.Domain.IRepositories;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class ConversationRepository : MongoRepository<Conversation>, IConversationRepository
    {
        private readonly IMongoCollection<Conversation> _conversations;

        public ConversationRepository(DbContext dbContext) : base(dbContext)
        {
            //_conversations = dbContext.GetCollection<Conversation>(CollectionType.Convesation);
        }

        public async Task<Conversation> GetConversationsByUserAsync(string userId)
        {
            return await _conversations.Find(c => c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task AddMessageAsync(string conversationId, Message message)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversationId);
            var update = Builders<Conversation>.Update.Push(c => c.Messages, message);
            await UpdateAsync(conversationId, new Conversation());
        }
    }
}

