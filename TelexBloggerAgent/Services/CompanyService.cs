using TelexBloggerAgent.IRepositories;
using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.Services
{
    public class CompanyService
    {
        private readonly IMongoRepository<Company> _companyRepository;
        private readonly UserService _userService;

        public CompanyService(IMongoRepository<Company> companyRepository, UserService userService)
        {
            _companyRepository = companyRepository;
            _userService = userService;
        }

        // Register a company and its channel (first request)
        public async Task<Company> RegisterCompanyAsync(string companyId, string channelId, string companyName)
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany != null)
                return existingCompany; // Company already exists

            var newCompany = new Company
            {
                Id = companyId,
                Name = companyName,
                CreatedAt = DateTime.UtcNow
            };

            await _companyRepository.CreateAsync(newCompany);

            // Register the company’s communication channel as a user
            await _userService.RegisterUserAsync(channelId, companyId, companyName, null, true);

            return newCompany;
        }

        // Get company by ID
        public async Task<Company?> GetCompanyByIdAsync(string companyId)
        {
            return await _companyRepository.GetByIdAsync(companyId);
        }
    }
}
