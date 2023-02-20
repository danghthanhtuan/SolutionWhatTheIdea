using System.Collections.Generic;

namespace SWTI.EasyNetQ.Hosepipe;

public interface IMessageWriter
{
    void Write(IEnumerable<HosepipeMessage> messages, QueueParameters parameters);
}
