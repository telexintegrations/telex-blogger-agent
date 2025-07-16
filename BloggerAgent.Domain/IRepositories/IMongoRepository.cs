using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IMongoRepository<T> where T : IEntity
    {
        Task<bool> CreateAsync(T document);
        Task<bool> DeleteAsync(string id);
        Task<List<T?>> GetAllAsync(Dictionary<string, string> filter = null);
        Task<T?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(string id, T document);
        Task<List<T?>> FilterAsync(Dictionary<string, string> filter);
        Task<List<T>> FilterByFieldAsync(string field, string value);
    }
}