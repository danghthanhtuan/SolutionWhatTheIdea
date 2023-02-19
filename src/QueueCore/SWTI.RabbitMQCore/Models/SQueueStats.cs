using System;
using System.Collections.Generic;
using System.Text;

namespace SWTI.RabbitMQCore.Models
{
    public class SQueueStats
    {
        public string Name { set; get; }
        /// <summary>
        ///     Messages count
        /// </summary>
        public ulong MessagesCount { get; set; }

        /// <summary>
        ///     Consumers count
        /// </summary>
        public ulong ConsumersCount { get; set; }
    }
}
