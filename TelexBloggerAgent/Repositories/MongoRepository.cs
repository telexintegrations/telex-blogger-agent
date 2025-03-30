using MongoDB.Driver;
using TelexBloggerAgent.Data;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IRepositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {
        private static readonly Dictionary<Type, string> CollectionMapping = new()
        {
            { typeof(User), CollectionType.User },
            { typeof(Conversation), CollectionType.Convesation },
            { typeof(Company), CollectionType.Company },
            { typeof(Message), CollectionType.Message },
            { typeof(Blog), CollectionType.Blog }
        };

        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext dbContext)
        {
            if (!CollectionMapping.TryGetValue(typeof(T), out string? collectionName))
                throw new ArgumentException($"No collection mapping found for type {typeof(T).Name}");

            _collection = dbContext.GetCollection<T>(collectionName);

            
        }

        // Create a new document
        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        // Get document by ID
        public async Task<T?> GetByIdAsync(string id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();  
        }

        // Get all documents
        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        
        public  IQueryable<T> AsQueryable()
        {
            return  _collection.AsQueryable();
        }

        // Pagination support
        public async Task<List<T>> GetPaginatedAsync(int page, int pageSize)
        {
            return await _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        // Update a document
        public async Task UpdateAsync(string id, UpdateDefinition<T> updateDefinition)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            await _collection.UpdateOneAsync(filter, updateDefinition);
        }


        // Soft Delete (Marks the document as deleted instead of removing it)
        public async Task SoftDeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var update = Builders<T>.Update.Set("IsDeleted", true);
            await _collection.UpdateOneAsync(filter, update);
        }

        // Delete a document
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }

}
