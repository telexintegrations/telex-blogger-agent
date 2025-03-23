using MongoDB.Driver;
using TelexBloggerAgent.Data;
using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class UserService
    {
        private readonly IMongoRepository<User> _userRepository;

        public UserService(IMongoRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUserAsync(string userId, string companyId, string? name = null, string? email = null, bool isChannel = false)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser != null)
                return existingUser; // User already exists

            var newUser = new User
            {
                Id = userId,  // Telex user/channel ID
                CompanyId = companyId,
                Name = name,
                Email = email,
            };

            await _userRepository.CreateAsync(newUser);
            return newUser;
        }

    }
}
