using MongoDB.Driver;
using TelexBloggerAgent.Data;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IRepositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {

        private readonly MongoDbContext _context;

        public MongoRepository(MongoDbContext context)
        {
            _context = context;
        }

        // Create a new document
        public async Task<bool> CreateAsync(T document)
        {
            var response = await _context.AddAsync<T>(document);
            return response.Status == "success";
        }

        public async Task<Document<T>> GetByIdAsync(string id)
        {
            var result = await _context.GetSingle<T>(id);
            return result.Status == "success" ? result.Data : default;
        }

        public async Task<List<Document<T>>> GetAllAsync(object filter = null)
        {
            var result = await _context.GetAll<T>(filter);

            if (result.Status == "error")
            {
                return null;
            }
            return result.Data;
        }
        
        public async Task<List<Document<T?>>> FilterAsync(object filter)
        {
            var result = await _context.GetAll<T>(filter);
            return result.Status == "success" ? result.Data : new List<Document<T>>();
        }

        // Update a new document
        public async Task<bool> UpdateAsync(string id, T document)
        {
            var response = await _context.UpdateAsync<T>(id, document);
            return response.Status == "success";
        }

        // Update a new document
        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _context.DeleteAsync<T>(id);
            return response.Status == "success";
        }

    }

}
