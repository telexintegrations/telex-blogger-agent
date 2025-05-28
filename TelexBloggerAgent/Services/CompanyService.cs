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


        public async Task<Company> AddCompanyAsync(Company company)
        {
            if (company.Id == null)
                throw new ArgumentNullException();

            var existingCompany = await _companyRepository.GetByIdAsync(company.Id);

            if (existingCompany != null)
            {
                throw new Exception("Company already exists");
            }

            var newCompany = new Company
            {
                Id = company.Id,
                Name = company.Name,
                Tone = company.Tone,
                TargetAudience = company.TargetAudience,
                Overview = company.Overview,
                Industry = company.Industry,   
            };

            /*var channelId = company.Users.FirstOrDefault().Id;

            var user = await _userService.GetUserAsync(channelId);

            if (user != null)
            {
                throw new DuplicateNameException();
            }
            var newUser = new User { Id = channelId };

            newCompany.Users.Add(newUser);

            try
            {
            }
            catch (Exception)
            {
                // Rollback: Delete the company if user creation fails
                await _userRepository.DeleteAsync(newUser.Id);
                throw;
            }*/
                
            // Register the company’s communication channel as a user
            await _companyRepository.CreateAsync(newCompany);

            return newCompany;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            if (company.Id == null)
                throw new ArgumentNullException();

            var companyDoc = await _companyRepository.GetByIdAsync(company.Id);

            if (companyDoc == null)
            {
                throw new Exception("Company not found exists");
            }

            var existingCompany = companyDoc.Data;

            existingCompany.Id = company.Id ?? existingCompany.Id;
            existingCompany.Name = company.Name ?? existingCompany.Name;
            existingCompany.Tone = company.Tone ?? existingCompany.Tone;
            existingCompany.TargetAudience = company.TargetAudience ?? existingCompany.TargetAudience;
            existingCompany.Overview = company.Overview ?? existingCompany.Overview;
            existingCompany.Industry = company.Industry ?? existingCompany.Industry;



            // Register the company’s communication channel as a user
            await _companyRepository.UpdateAsync("", existingCompany);

            return existingCompany;
        }

        //public async Task<Company?> GetCompanyByIdAsync(string companyId)
        //{
        //    if (string.IsNullOrEmpty(companyId))
        //    {
        //        throw new ArgumentNullException(nameof(companyId));
        //    }

        //    return await _companyRepository.FilterAsync(companyId);
        //}
    }
}
