using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IBlogRepository : ITelexRepository<Blog>
    {
        Task<bool> AddBlogAsync(Blog company);
        Task<bool> UpdateBlogAsync(Dictionary<string, object> blogFieldsToUpdate, string topic);
    }
}