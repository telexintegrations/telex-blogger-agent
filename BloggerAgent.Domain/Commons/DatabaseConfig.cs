using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BloggerAgent.Domain.Commons
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

    }

}
