using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IOrganizationRepository : IMongoRepository<Company>
    {
        Task<bool> CreateCompanyAsync(Company company);
        Task<bool> UpdateCompanyAsync(Company company);
    }
}