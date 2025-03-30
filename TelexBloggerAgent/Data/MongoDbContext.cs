using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TelexBloggerAgent.Data
{
    public class MongoDbContext
    {
        public IMongoDatabase Database { get; }

        public MongoDbContext(IOptions<MongoDbConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            Database = client.GetDatabase(config.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(in string name) => Database.GetCollection<T>(name);
    }
}
