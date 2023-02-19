namespace SWTI.RedisProvider
{

    public class SWTIRedis : IDisposable
    {
        public RedisClient RedisClient;

        public SWTIRedis(string RedisServerAddress,
             string IsTwemproxy,
             string RedisTimeOut)
        {
            RedisClient = new RedisClient(RedisServerAddress, IsTwemproxy, RedisTimeOut);
        }

        public void Dispose()
        {
            RedisClient.Dispose();
        }
    }
}
