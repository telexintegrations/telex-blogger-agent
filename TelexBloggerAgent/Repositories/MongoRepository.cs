using MongoDB.Driver;
using TelexBloggerAgent.Data;

namespace TelexBloggerAgent.IRepositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext dbContext, string collectionName)
        {
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

        // Update a document
        public async Task UpdateAsync(string id, UpdateDefinition<T> updateDefinition)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            await _collection.UpdateOneAsync(filter, updateDefinition);
        }

        // Delete a document
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }

}
