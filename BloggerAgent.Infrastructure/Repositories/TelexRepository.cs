using MongoDB.Driver;
using BloggerAgent.Domain.Data;
using BloggerAgent.Application.Helpers;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainHelper;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class TelexRepository<T> : ITelexRepository<T> where T : IEntity
    {
        private readonly DbContext _context;
        private readonly TaskContextAccessor _contextAccessor;
        public TelexRepository(DbContext context)
        {
            _context = context;
        }

        public string OrgId => _context.TaskContext.OrgId;

        public async Task<bool> CreateAsync(T document)
        {
            var response = await _context.AddAsync<T>(document);
            return response.Status == "success";
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var result = await _context.GetSingle<T>(id);
            return result.Status == "success" ? result.Data : default;
        }

        public async Task<List<T>> GetAllAsync(Dictionary<string, object> filter = null)
        {
            var result = await _context.GetAll<T>(filter); 
            
            if (result.Status != "success" || result.Data == null)
                return new List<T>();

            return result.Data;

        }

        public async Task<List<T?>> FilterAsync(Dictionary<string, object> filter)
        {
            var result = await _context.GetAll<T>(filter);
            return result.Status == "success" ? result.Data : new List<T?>();
        }

        public async Task<bool> UpdateAsync(string id, T document)
        {
            var response = await _context.UpdateAsync<T>(id, document);
            return response.Status == "success";
        }
        
        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _context.DeleteAsync<T>(id);
            return response.Status == "success";
        }

        public async Task<List<T>> FilterByFieldAsync(string field, string value)
        {
            var filter = new Dictionary<string, object> { [field] = value };
            return await FilterAsync(filter);
        }

    }

}
