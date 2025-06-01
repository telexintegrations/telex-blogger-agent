using MongoDB.Driver;
using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Application.IServices;
using BloggerAgent.Domain.Models;

namespace BloggerAgent.Infrastructure.Services
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
