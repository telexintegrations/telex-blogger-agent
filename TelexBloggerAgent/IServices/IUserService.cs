using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IServices
{
    public interface IUserService
    {
        Task<User> AddUserAsync(string userId, string companyId, bool isChannel = true);
        Task<User> GetUserAsync(string userId);
    }
}