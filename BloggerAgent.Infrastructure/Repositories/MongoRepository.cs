using MongoDB.Driver;
using BloggerAgent.Domain.Data;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {

        private readonly DbContext _context;

        public MongoRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(T document)
        {
            var response = await _context.AddAsync<T>(document);
            return response.Status == "success";
        }

        public async Task<Document<T>> GetByIdAsync(string id)
        {
            var result = await _context.GetSingle<T>(id);
            return result.Status == "success" ? result.Data : null;
        }

        public async Task<List<Document<T>>> GetAllAsync(object filter = null)
        {
            var result = await _context.GetAll<T>(filter);

            return result.Status == "success" ? result.Data : null;
        }

        public async Task<List<Document<T?>>> FilterAsync(object filter)
        {
            var result = await _context.GetAll<T>(filter);
            return result.Status == "success" ? result.Data : null;
        }

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
