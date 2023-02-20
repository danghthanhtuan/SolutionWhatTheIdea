using StackExchange.Redis;

namespace SWTI.RedisProvider
{
    /// <summary>
    /// Nơi chứa connection Redis
    /// </summary>
    /// <author>nguyenpt8</author>
    internal class RedisStore
    {
        private readonly ConfigurationOptions _configurationOptions;
        private ConnectionMultiplexer connection;

        public RedisStore(string RedisServerAddress,
             string IsTwemproxy,
             string RedisTimeOut
             )
        {
            _configurationOptions = new ConfigurationOptions
            {
                EndPoints = { RedisServerAddress },
                Proxy = string.IsNullOrEmpty(IsTwemproxy) || IsTwemproxy == "false"
                ? Proxy.None
                : Proxy.Twemproxy,
                //Proxy = Proxy.None,
                ConnectTimeout = string.IsNullOrEmpty(RedisTimeOut)
                ? 5000
                : Convert.ToInt32(RedisTimeOut),
                AbortOnConnectFail = false,
                ResponseTimeout = string.IsNullOrEmpty(RedisTimeOut)
                ? 5000
                : Convert.ToInt32(RedisTimeOut),
                //AsyncTimeout = string.IsNullOrEmpty(RedisTimeOut)
                //? 5000
                //: Convert.ToInt32(RedisTimeOut),
                //ConnectRetry = 10,
            };
        }

        public async Task<IDatabase> RedisConnectionAsync()
        {
            if (connection == null)
            {
                connection = await ConnectionMultiplexer.ConnectAsync(_configurationOptions);
            }

            return connection.GetDatabase();
        }

        public IDatabase RedisConnection()
        {
            if (connection == null)
            {
                connection = ConnectionMultiplexer.Connect(_configurationOptions);
            }

            return connection.GetDatabase();
        }

        public void Dispose()
        {
            connection.Dispose();
            connection.Close();
        }
    }
}
