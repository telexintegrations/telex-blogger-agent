using MongoDB.Driver;
using TelexBloggerAgent.Data;
using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoRepository<User> _userRepository;

        public UserService(IMongoRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AddUserAsync(string userId, string companyId, bool isChannel)
        {
            if (userId == null || companyId == null)
                throw new ArgumentNullException();

            var newUser = new User
            {
                Id = userId,  // Telex user/channel ID
            };

            await _userRepository.CreateAsync(newUser);
            return newUser;
        }

        public async Task<User> GetUserAsync(string userId)
        {
            //return await _userRepository.GetByIdAsync(userId);
            return null;
        }
    }
}
