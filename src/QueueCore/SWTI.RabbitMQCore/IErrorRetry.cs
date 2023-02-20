using EasyNetQ.SystemMessages;
using SWTI.EasyNetQ.Hosepipe;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    public interface IErrorRetry
    {
        Task ExecuteRetryAsync(CancellationToken cancellationToken = default);
        Task ExecuteRetryByQueueName(string queueName, CancellationToken cancellationToken = default);
        string GetDeadQueueName(Error error);
        Task PublishToDeadQueue(Error error, CancellationToken cancellationToken = default);
        Task PublishToRetryMessage(Error error, CancellationToken cancellationToken = default);
        Task RepublishErrorAsync(Error error, QueueParameters parameters, CancellationToken cancellationToken);
        Task RetryErrorsAsync(IEnumerable<HosepipeMessage> rawErrorMessages, CancellationToken cancellationToken);
    }
}