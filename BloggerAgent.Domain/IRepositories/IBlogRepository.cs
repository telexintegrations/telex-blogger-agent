using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IBlogRepository : IMongoRepository<Blog>
    {
        Task<bool> AddBlogAsync(Blog company);
        Task<bool> UpdateBlogAsync(Blog company, string orgId);
    }
}