using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.Data;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly DbContext _db;
        private static Dictionary<string, string> _keyCache = new();

        public ApiKeyRepository(DbContext db)
        {
            _db = db;
        }

        public async Task<string> GetKeyByOrgIdAsync(string orgId)
        {
            var result = await _db.GetAll<ApiKeyRecord>();

            return result.Data?.FirstOrDefault()?.Key;
        }

        public async Task<bool> SaveKeyAsync(string apiKey)
        {
            if (_keyCache.TryGetValue(_db.TaskContext.OrgId, out var key))
            {

            }

            var filter = new { tag = "ApiKey" };
            var result = await _db.AddAsync(new ApiKey { Key = _db.TaskContext.OrgId});

            return result.Status == "success";
        }
    }

    public class ApiKeyRecord
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Key { get; set; }
    }
}
