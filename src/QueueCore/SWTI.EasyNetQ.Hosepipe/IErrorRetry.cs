using System.Collections.Generic;

namespace SWTI.EasyNetQ.Hosepipe;

public interface IErrorRetry
{
    void RetryErrors(IEnumerable<HosepipeMessage> rawErrorMessages, QueueParameters parameters);
}
