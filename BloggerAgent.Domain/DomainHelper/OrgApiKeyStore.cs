using BloggerAgent.Domain.IRepositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.DomainHelper
{
    public class OrgApiKeyStore 
    {
        private readonly IMemoryCache _cache;
        private readonly IApiKeyRepository _repository; // Your DB access abstraction
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public OrgApiKeyStore(IMemoryCache cache, IApiKeyRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<string> GetApiKeyAsync(string orgId)
        {
            if (_cache.TryGetValue(orgId, out string cachedKey))
                return cachedKey;

            var keyFromDb = await _repository.GetKeyByOrgIdAsync(orgId);
            if (keyFromDb == null)
                throw new InvalidOperationException("No API key found for organization");

            _cache.Set(orgId, keyFromDb, _cacheDuration);
            return keyFromDb;
        }

        public async Task<string> AddApiKeyAsync(string orgId, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(orgId) || string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Org ID and API key must be provided");

            // Persist the key in DB (optional, if you're storing it permanently)
            await _repository.SaveKeyAsync(apiKey);

            // Update cache
            _cache.Set(orgId, apiKey, _cacheDuration);

            return apiKey;
        }
    }
}
