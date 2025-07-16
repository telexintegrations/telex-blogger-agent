using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using BloggerAgent.Domain.Models;
using BloggerAgent.Domain.Commons;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.IRepositories;

namespace BloggerAgent.Domain.Data
{
    public class DbContext 
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly HttpHelper _httpHelper;
        private const string _collectionName = "test_collection";
        private readonly string tagName;

        public DbContext(IOptions<TelexApiSettings> options, HttpHelper httphelper)
        {
            _baseUrl = options.Value.BaseUrl;
            _apiKey = options.Value.ApiKey;
            _httpHelper = httphelper;

            if (_apiKey == null || _baseUrl == null)
            {
                throw new ArgumentNullException(nameof(_apiKey) ?? nameof(_baseUrl));
            }
            _baseUrl += "/agent_db/collections";
        }

        public async Task<TelexApiResponse<T?>> CreateCollection<T>()
        {
            var apiRequest = new ApiRequest()
            {
               Method = HttpMethod.Post,
               Url = _baseUrl,
               Body = new
               {
                   collection_name = _collectionName,
               },
               Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }

         
        public async Task<TelexApiResponse<List<T?>>> GetAll<T>(Dictionary<string, string> filter = null)
        {
            if (filter == null)
            {
                filter = new Dictionary<string, string>();
            }

            filter["tag_name"] = CollectionType.ResolveTagName<T>();
           
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{_collectionName}/documents",
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
                return TelexApiResponse<List<T>>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<List<T>>.ExtractResponse(responseContent);

        }

        private Dictionary<string, string> PrepareOrgHeader()
        {
            return new Dictionary<string, string>()
            {
               {TelexApiSettings.Header, _apiKey}
            };
        }

        public async Task<TelexApiResponse<T?>> GetSingle<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{_collectionName}/documents/{id}",               
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
           

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);

        }


        public async Task<TelexApiResponse<T?>> AddAsync<T>(T document) where T : IEntity
        {
            string tagName = CollectionType.ResolveTagName<T>();
            document.TagName = tagName;

            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = $"{_baseUrl}/{_collectionName}/documents",
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
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }
         
        public async Task<TelexApiResponse<T?>> UpdateAsync<T>(string id, object document)
        {
            
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Put,
                Url = $"{_baseUrl}/{_collectionName}/documents/{id}",
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
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }

         
       
        public async Task<TelexApiResponse<T?>> DeleteAsync<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Delete,
                Url = $"{_baseUrl}/{_collectionName}/documents/{id}",
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }

    }
}
