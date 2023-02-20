using System.Collections.Generic;

namespace SWTI.EasyNetQ.Hosepipe;

public interface IQueueInsertion
{
    void PublishMessagesToQueue(IEnumerable<HosepipeMessage> messages, QueueParameters parameters);
}
