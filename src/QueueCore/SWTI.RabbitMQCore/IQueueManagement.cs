using EasyNetQ;
using EasyNetQ.Management.Client.Model;
using SWTI.RabbitMQCore.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    public interface IQueueManagement
    {
        Task<IEnumerable<Queue>> GetAllQueue(CancellationToken cancellationToken);
        Task<IEnumerable<SQueueStats>> GetStatusAllErrorQueue(CancellationToken cancellationToken);
        Task<SQueueStats> GetStatusFromQueueName(string queueName, bool durable,
            bool exclusive,
            bool autoDelete, CancellationToken cancellationToken);
    }
}