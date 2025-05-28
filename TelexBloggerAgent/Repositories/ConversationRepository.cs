using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TelexBloggerAgent.Data;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Repositories
{
    public class ConversationRepository : MongoRepository<Conversation>, IConversationRepository
    {
        private readonly IMongoCollection<Conversation> _conversations;

        public ConversationRepository(MongoDbContext dbContext) : base(dbContext)
        {
            _conversations = dbContext.GetCollection<Conversation>(CollectionType.Convesation);
        }

        public async Task<Conversation> GetConversationsByUserAsync(string userId)
        {
            return await _conversations.Find(c => c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task AddMessageAsync(string conversationId, Message message)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversationId);
            var update = Builders<Conversation>.Update.Push(c => c.Messages, message);
            await UpdateAsync(conversationId, update);
        }
    }
}

