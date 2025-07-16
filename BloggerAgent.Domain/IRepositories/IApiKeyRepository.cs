using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.IRepositories
{
    public interface IApiKeyRepository
    {
        Task<string> GetKeyByOrgIdAsync(string orgId);
        Task<bool> SaveKeyAsync(string apiKey);
    }
}
