using MongoDB.Driver;

namespace TelexBloggerAgent.IRepositories
{
    public interface IMongoRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(string id);
        Task<List<T>> GetAllAsync();
        IQueryable<T> AsQueryable();
        Task<T?> GetByIdAsync(string id);
        Task UpdateAsync(string id, UpdateDefinition<T> updateDefinition);
    }
}