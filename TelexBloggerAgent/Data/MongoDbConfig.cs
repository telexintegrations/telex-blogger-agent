using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TelexBloggerAgent.Data
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; }
        
    }

}
