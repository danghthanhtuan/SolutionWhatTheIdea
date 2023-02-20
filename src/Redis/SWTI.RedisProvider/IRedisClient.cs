using StackExchange.Redis;

namespace SWTI.RedisProvider
{
    public interface IRedisClient
    {
        bool DeleteFromKey(string key);

        HashEntry[] GetHashFromKey(string key);

        byte[] GetValueFromKey(string key);

        string GetValueStringFromKey(string key);

        bool SetHashFromKey(string key, HashEntry[] hash, TimeSpan? expireTime = null);

        bool SetValueFromKey(string key, byte[] value, TimeSpan? expireTime = null);

        bool SetValueStringFromKey(string key, string value, TimeSpan? expireTime = null);

        Task<bool> SetValueFromKeyAsync(string key, byte[] value, TimeSpan? expireTime = null);

        Task<RedisValue> GetValueFromKeyAsync(string key);

        Task<bool> SetValueStringFromKeyAsync(string key, string value, TimeSpan? expireTime = null);

        Task<bool> DeleteFromKeyAsync(string key);

        Task<T> GetByJsonAsync<T>(string key);

        Task<bool> SetByJsonAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None);

        //Task<T> GetByProtoAsync<T>(string key);

        //Task<bool> SetByProtoAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None);

        Task<T> GetByMessagePackAsync<T>(string key, CommandFlags flags, CancellationToken cancellationToken);

        Task<bool> SetByMessagePackAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.FireAndForget, CancellationToken cancellationToken = default);
        Task<T> GetByProtoAsync<T>(string key, CommandFlags flags, CancellationToken cancellationToken);
        Task<bool> SetByProtoAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);
    }
}
