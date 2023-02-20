using EasyNetQ;
using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;
using SWTI.RabbitMQCore.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    public class QueueManagement : IQueueManagement, IDisposable
    {
        private readonly ManagementClient _managementClient;
        private readonly RabbitMqConfigration _rabbitMqConfigration;
        private readonly IBus _bus;

        public QueueManagement(RabbitMqConfigration rabbitMqConfigration, IBus bus)
        {
            _rabbitMqConfigration = rabbitMqConfigration;
            _managementClient = new ManagementClient(_rabbitMqConfigration.Hosts[0]?.Host
                , _rabbitMqConfigration.UserName
                , _rabbitMqConfigration.Password
                , _rabbitMqConfigration.Hosts[0].Port
                );
            _bus = bus;
        }

        public void Dispose()
        {
            _managementClient.Dispose();
        }

        public async Task<IEnumerable<Queue>> GetAllQueue(CancellationToken cancellationToken)
        {
            return await _managementClient.GetQueuesAsync(cancellationToken);
        }

        public async Task<SQueueStats> GetStatusFromQueueName(string queueName, bool durable,
            bool exclusive,
            bool autoDelete, CancellationToken cancellationToken)
        {
            await _bus.Advanced.QueueDeclarePassiveAsync(queueName, cancellationToken);
            var queue = await _bus.Advanced.QueueDeclareAsync(queueName, durable, exclusive, autoDelete, cancellationToken);
            var data = await _bus.Advanced.GetQueueStatsAsync(queue.Name, cancellationToken);
            return new SQueueStats { Name = queueName, ConsumersCount = data.ConsumersCount, MessagesCount = data.MessagesCount };
        }

        public async Task<IEnumerable<SQueueStats>> GetStatusAllErrorQueue(CancellationToken cancellationToken)
        {
            List<Task<SQueueStats>> tasks = new List<Task<SQueueStats>>();
            foreach (var item in Storage.DeadQueueNameStore())
            {
                tasks.Add(GetStatusFromQueueName(item, false, false, false, cancellationToken));
                //yield return await GetStatusFromQueueName(item, cancellationToken);
            }
            foreach (var item in Storage.ErrorQueueNameStore())
            {
                tasks.Add(GetStatusFromQueueName(item, true, false, false, cancellationToken));
                //yield return await GetStatusFromQueueName(item, cancellationToken);
            }
            await Task.WhenAll(tasks);
            List<SQueueStats> list = new List<SQueueStats>();
            foreach (var item in tasks)
            {
                list.Add(await item);
            }
            return list;
        }
    }
}