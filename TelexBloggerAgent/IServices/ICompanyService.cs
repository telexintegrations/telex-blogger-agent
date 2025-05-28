using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IServices
{
    public interface ICompanyService
    {
        Task<Company> AddCompanyAsync(Company company);
        //Task<Company?> GetCompanyByIdAsync(string companyId);
    }
}