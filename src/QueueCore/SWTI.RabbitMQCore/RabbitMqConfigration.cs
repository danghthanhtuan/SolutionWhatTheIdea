using System.Collections.Generic;

namespace SWTI.RabbitMQCore
{
    public class RabbitMqConfigration
    {
        public List<RabbitMqHost> Hosts { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }
        public ushort Timeout { get; set; }
        public ushort PrefetchCount { get; set; }
        public bool IsConsume { get; set; }
        public string ErrorQueueNaming { set; get; }
        public string SubscriptionIdPrefix { set; get; }
        public int MaxNumberOfMessageRetry { get; set; }
        public int MaxRetry { get; set; }
    }

    public class RabbitMqHost
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
    }
}