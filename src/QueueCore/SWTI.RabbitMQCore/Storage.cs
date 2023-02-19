using System.Collections.Generic;

namespace SWTI.RabbitMQCore
{
    internal static class Storage
    {
        public static List<string> QueueNameStore { get; set; }

        public static IEnumerable<string> ErrorQueueNameStore()
        {
            foreach (var store in QueueNameStore)
            {
                yield return store + Const.ErrorName;
            }
        }

        public static IEnumerable<string> DeadQueueNameStore()
        {
            foreach (var store in QueueNameStore)
            {
                yield return store + Const.DeadName;
            }
        }
    }
}