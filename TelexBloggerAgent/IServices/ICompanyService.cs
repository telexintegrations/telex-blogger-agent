using TelexBloggerAgent.Models;

namespace TelexBloggerAgent.IServices
{
    public interface ICompanyService
    {
        Task<Company> AddCompanyAsync(string companyId, string channelId, Company company);
        Task<Company?> GetCompanyByIdAsync(string companyId);
    }
}