using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;
using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.IServices;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IMongoRepository<Company> _companyRepository;
        private readonly IMongoRepository<User> _userRepository;
        private readonly IUserService _userService;

        public CompanyService(IMongoRepository<Company> companyRepository, IMongoRepository<User> userRepository, IUserService userService)
        {
            _companyRepository = companyRepository;
            _userService = userService;
            _userRepository = userRepository;
        }


        public async Task<Company> AddCompanyAsync(string companyId, string channelId, Company company)
        {
            if (companyId == null)
                throw new ArgumentNullException();

            var newCompany = new Company
            {
                Id = companyId,
                Name = company.Name,
                Tone = company.Tone,
                TargetAudience = company.TargetAudience,
                Overview = company.Overview,
                Industry = company.Industry,                
            };

            var user = await _userService.GetUserAsync(channelId);

            if (user != null)
            {
                throw new DuplicateNameException();
            }

            var newUser = await _userService.AddUserAsync(channelId, newCompany.Id);

            newCompany.Users.Add(newUser);


            try
            {
                // Register the company’s communication channel as a user
                await _companyRepository.CreateAsync(newCompany);
            }
            catch (Exception)
            {
                // Rollback: Delete the company if user creation fails
                await _userRepository.DeleteAsync(newUser.Id);
                throw;
            }

            return newCompany;
        }

        public async Task<Company?> GetCompanyByIdAsync(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _companyRepository.GetByIdAsync(companyId);
        }
    }
}
