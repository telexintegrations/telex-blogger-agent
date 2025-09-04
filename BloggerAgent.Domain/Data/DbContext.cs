using Microsoft.Extensions.Options;
using BloggerAgent.Domain.Models;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;
using BloggerAgent.Domain.Commons.Options;
using BloggerAgent.Domain.Commons.constants;
using BloggerAgent.Domain.Commons.DataEntities;
using Microsoft.Extensions.Logging;

namespace BloggerAgent.Domain.Data
{
    public class DbContext 
    {
        private readonly string _baseUrl;
        private readonly HttpHelper _httpHelper;
        private const string CollectionName = "blogger_agent_db";
        private readonly TaskContextAccessor _taskContextAccessor;
        private readonly ILogger<DbContext> _logger;

        public DbContext(IOptions<TelexApiSettings> options, HttpHelper httphelper, TaskContextAccessor contextAccessor, ILogger<DbContext> logger)
        {
            _taskContextAccessor = contextAccessor;
            _httpHelper = httphelper;
            _logger = logger;
            _baseUrl = BuildBaseUrl() ?? 
                $"{options.Value.BaseUrl.TrimEnd('/')}/agent_db/collections";
        }

        public TaskContext TaskContext => 
            _taskContextAccessor.GetTaskContext();

        private string BuildBaseUrl()
        {
            var context = TaskContext;
            if (context?.CallbackUrl != null && context.CallbackUrl.Contains("staging"))
            {
                return "https://api.staging.telex.im/api/v1/agent_db/collections";
            }

            return null;
        }

        public async Task<TelexApiResponse<T?>> CreateCollection<T>()
        {
            var apiRequest = new ApiRequest()
            {
               Method = HttpMethod.Post,
               Url = _baseUrl,
               Body = new
               {
                   collection_name = CollectionName,
               },
               Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<T?>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T?>.ExtractResponse(responseContent);
        }

         
        public async Task<TelexApiResponse<List<T?>>> GetAll<T>(Dictionary<string, object> filter = null)
        {
            if (filter == null)
            {
                filter = new Dictionary<string, object>();
            }

            filter["tag"] = CollectionType.ResolveTagName<T>();
           
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{CollectionName}/documents",
                Body = new
                {
                    Filter = filter
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<List<T?>>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<List<T?>>.ExtractResponse(responseContent);

        }


        public async Task<TelexApiResponse<T?>> GetSingle<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",               
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
           

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<T?>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T?>.ExtractResponse(responseContent);

        }


        public async Task<TelexApiResponse<T?>> AddAsync<T>(T document) where T : IEntity
        {
            string tagName = CollectionType.ResolveTagName<T>();
            document.Tag = tagName;

            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = $"{_baseUrl}/{CollectionName}/documents",
                Body = new
                {
                    Document = document 
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<T?>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T?>.ExtractResponse(responseContent);
        }
         
        public async Task<TelexApiResponse<T?>> UpdateAsync<T>(string id, object document)
        {
            
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Put,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",
                Body = new
                {
                    Document = document
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<T?>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T?>.ExtractResponse(responseContent);
        }
         
       
        public async Task<TelexApiResponse<T?>> DeleteAsync<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Delete,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<T?>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T?>.ExtractResponse(responseContent);
        }

        private Dictionary<string, string> PrepareOrgHeader()
        {
            if (string.IsNullOrEmpty(TaskContext?.AuthToken))
            {
                _logger.LogError("Auth Token not found in Task Context");
                throw new KeyNotFoundException(nameof(TaskContext.AuthToken));
            }

            return new Dictionary<string, string>()
            {
               {TelexApiSettings.Header, TaskContext.AuthToken}
            };
        }

    }
}
