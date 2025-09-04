using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IBlogRepository : ITelexRepository<Blog>
    {
        Task<Blog> GetBlogAsync(string topic = null);
        Task<bool> AddBlogAsync(Blog company);
        Task<bool> UpdateBlogAsync(Dictionary<string, object> blogFieldsToUpdate, string topic);
    }
}