using Newtonsoft.Json;
using StackExchange.Profiling;
using StackExchange.Redis;

namespace SWTI.RedisProvider
{
    /// <summary>
    /// Nơi chứa get set repo cho Redis
    /// http://stackoverflow.com/questions/25898333/how-to-add-generic-list-to-redis-via-stackexchange-redis
    /// </summary>
    /// <author>nguyenpt8</author>
    public class RedisClient : IRedisClient, IDisposable
    {
        private readonly RedisStore RedisStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisClient"/> class.
        /// </summary>
        /// <param name="RedisStore">The RedisStore.</param>
        public RedisClient(string RedisServerAddress,
             string IsTwemproxy,
             string RedisTimeOut)
        {
            RedisStore = new RedisStore(RedisServerAddress, IsTwemproxy, RedisTimeOut);
        }

        public byte[] GetValueFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            return RedisStore.RedisConnection().StringGet(key);
        }

        public bool SetValueFromKey(string key, byte[] value, TimeSpan? expireTime = null)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null) return false;

            if (expireTime.HasValue)
            {
                RedisStore.RedisConnection().StringSet(key, value, expireTime);
            }
            else
            {
                RedisStore.RedisConnection().StringSet(key, value);
            }
            return true;
        }

        public async Task<bool> SetValueFromKeyAsync(string key, byte[] value, TimeSpan? expireTime = null)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            if (expireTime.HasValue)
            {
                return await (await RedisStore.RedisConnectionAsync()).StringSetAsync(key, value, expireTime);
            }
            else
            {
                return await (await RedisStore.RedisConnectionAsync()).StringSetAsync(key, value);
            }
            return true;
        }

        public string GetValueStringFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            return RedisStore.RedisConnection().StringGet(key);
        }

        public async Task<RedisValue> GetValueFromKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return default;

            return await (await RedisStore.RedisConnectionAsync()).StringGetAsync(key);
        }

        public async Task<RedisValue> GetObjectFromKeyAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return default;

            return await (await RedisStore.RedisConnectionAsync()).StringGetAsync(key);
        }

        public bool SetValueStringFromKey(string key, string value, TimeSpan? expireTime = null)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            if (expireTime.HasValue)
            {
                RedisStore.RedisConnection().StringSet(key, value, expireTime);
            }
            else
            {
                RedisStore.RedisConnection().StringSet(key, value);
            }
            return true;
        }

        public async Task<bool> SetValueStringFromKeyAsync(string key, string value, TimeSpan? expireTime = null)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            if (expireTime.HasValue)
            {
                return await (await RedisStore.RedisConnectionAsync()).StringSetAsync(key, value, expireTime);
            }
            else
            {
                return await (await RedisStore.RedisConnectionAsync()).StringSetAsync(key, value);
            }
        }

        public bool DeleteFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(key)) return false;

            RedisStore.RedisConnection().KeyDelete(key);
            return true;
        }

        public Task<bool> DeleteFromKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(key)) return Task.FromResult(false);

            return RedisStore.RedisConnection().KeyDeleteAsync(key);
        }

        public HashEntry[] GetHashFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(key)) return null;

            return RedisStore.RedisConnection().HashGetAll(key);
        }

        public bool SetHashFromKey(string key, HashEntry[] hash, TimeSpan? expireTime = null)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(key)) return false;
            RedisStore.RedisConnection().HashSet(key, hash);
            if (expireTime.HasValue)
            {
                RedisStore.RedisConnection().KeyExpire(key, expireTime);
            }

            return true;
        }

        public void Dispose()
        {
            RedisStore.Dispose();
        }

        /// <summary>
        /// Sử dụng Json để Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetByJsonAsync<T>(string key)
        {

            var value = await (await RedisStore.RedisConnectionAsync()).StringGetAsync(key);
            if (value.HasValue)
                return JsonConvert.DeserializeObject<T>(value);
            else
            {
                return default;
            }
        }

        public async Task<bool> SetByJsonAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None)
        {
            var json = JsonConvert.SerializeObject(value);
            return await (await RedisStore.RedisConnectionAsync()).StringSetAsync(key, json, expireTime, flags: flags);
        }

        public async Task SetByHashAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None)
        {
            var hash = value.ToHashEntries();
            var hashset = (await RedisStore.RedisConnectionAsync()).HashSetAsync(key, hash, flags: flags);
            var expire = (await RedisStore.RedisConnectionAsync()).KeyExpireAsync(key, expireTime, flags);
            await Task.WhenAll(hashset, expire);
        }

        public async Task<T> GetByHashAllAsync<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            using (MiniProfiler.Current.Step(nameof(GetByHashAllAsync))) ;
            var redisHash = await (await RedisStore.RedisConnectionAsync()).HashGetAllAsync(key, flags);
            return redisHash.ConvertHashToOjbect<T>();
        }

        public Task<T> GetByMessagePackAsync<T>(string key, CommandFlags flags, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetByMessagePackAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.FireAndForget, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByProtoAsync<T>(string key, CommandFlags flags, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetByProtoAsync<T>(string key, T value, TimeSpan? expireTime = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
