using EasyNetQ;
using EasyNetQ.Management.Client;
using SWTI.Interfaces.IQueue;
using SWTI.Queue;
using SWTL.Models.Queues;
using EasyNetQ.Topology;

namespace SWTI.QueueCore
{
    public class QueueManagement : IQueueManagement, IDisposable
    {
        private readonly ManagementClient _managementClient;

        private readonly RabbitMqConfigration _rabbitMqConfigration;

        private readonly IBus _bus;

        public QueueManagement(RabbitMqConfigration rabbitMqConfigration, IBus bus)
        {
            _rabbitMqConfigration = rabbitMqConfigration;
            _managementClient = new ManagementClient(_rabbitMqConfigration.Hosts[0]?.Host, _rabbitMqConfigration.UserName, _rabbitMqConfigration.Password, _rabbitMqConfigration.Hosts[0].Port);
            _bus = bus;
        }

        public void Dispose()
        {
            _managementClient.Dispose();
        }

        public async Task<IEnumerable<EasyNetQ.Management.Client.Model.Queue>> GetAllQueue(CancellationToken cancellationToken)
        {
            return await _managementClient.GetQueuesAsync(cancellationToken);
        }

        public async Task<SQueueStats> GetStatusFromQueueName(string queueName, bool durable, bool exclusive, bool autoDelete, CancellationToken cancellationToken)
        {
            await _bus.Advanced.QueueDeclarePassiveAsync(queueName, cancellationToken);
           // EasyNetQ.Topology.Queue queue = await _bus.Advanced.QueueDeclareAsync(queueName, durable, exclusive, autoDelete, cancellationToken);
            QueueStats data = await _bus.Advanced.GetQueueStatsAsync(queueName);
            return new SQueueStats
            {
                Name = queueName,
                ConsumersCount = data.ConsumersCount,
                MessagesCount = data.MessagesCount
            };
        }

        public async Task<IEnumerable<SQueueStats>> GetStatusAllErrorQueue(CancellationToken cancellationToken)
        {
            List<Task<SQueueStats>> tasks = new List<Task<SQueueStats>>();
            foreach (string item3 in Storage.DeadQueueNameStore())
            {
                tasks.Add(GetStatusFromQueueName(item3, durable: false, exclusive: false, autoDelete: false, cancellationToken));
            }

            foreach (string item2 in Storage.ErrorQueueNameStore())
            {
                tasks.Add(GetStatusFromQueueName(item2, durable: true, exclusive: false, autoDelete: false, cancellationToken));
            }

            await Task.WhenAll(tasks);
            List<SQueueStats> list = new List<SQueueStats>();
            foreach (Task<SQueueStats> item in tasks)
            {
                List<SQueueStats> list2 = list;
                list2.Add(await item);
            }

            return list;
        }
    }
}
