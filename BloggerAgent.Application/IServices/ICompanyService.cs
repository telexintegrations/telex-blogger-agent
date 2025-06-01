using BloggerAgent.Domain.Models;

namespace BloggerAgent.Application.IServices
{
    public interface ICompanyService
    {
        Task<Company> AddCompanyAsync(Company company);
        //Task<Company?> GetCompanyByIdAsync(string companyId);
    }
}