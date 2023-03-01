using MongoDB.Driver;

namespace SWTI.Interfaces.IProviders
{
    public interface IMongoDBContext
    {
        public IMongoCollection<T> Collection<T>(string dbName, string collectionName);
        public IMongoCollection<T> Collection<T>(string collectionName);
    }
}
