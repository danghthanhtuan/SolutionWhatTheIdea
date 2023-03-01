using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SWTI.Configurations;
using SWTI.Interfaces.IProviders;

namespace SWTI.Providers
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly string _connectionString;
        private MongoClient mongoClient;
        private readonly ILogger<MongoDBContext> _logger;
        private readonly string _databaseName;

        public MongoDBContext(IOptions<MongoConnectionStrings> options
            , ILogger<MongoDBContext> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _logger = logger;
            _databaseName = options.Value.DatabaseName;
        }

        public IMongoClient Client()
        {
            if (mongoClient != null)
            {
                return mongoClient;
            }
            mongoClient = new MongoClient(_connectionString);
            _logger.LogInformation("SenpayIdDBContext new MongoClient(_connectionString) done");
            return mongoClient;
        }

        public IMongoDatabase Database(string dbName) => Client().GetDatabase(DatabaseName);

        public IMongoCollection<T> Collection<T>(string dbName, string collectionName) => Database(dbName).GetCollection<T>(collectionName);

        public IMongoCollection<T> Collection<T>(string collectionName) => Database(_databaseName).GetCollection<T>(collectionName);

        public string DatabaseName => _databaseName;
    }
}
