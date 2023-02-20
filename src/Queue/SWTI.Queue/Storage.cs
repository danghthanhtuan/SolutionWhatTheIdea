namespace SWTI.QueueCore
{
    internal static class Storage
    {
        public static List<string> QueueNameStore { get; set; }

        public static IEnumerable<string> ErrorQueueNameStore()
        {
            foreach (string store in QueueNameStore)
            {
                yield return store + "_error";
            }
        }

        public static IEnumerable<string> DeadQueueNameStore()
        {
            foreach (string store in QueueNameStore)
            {
                yield return store + "_error_error";
            }
        }
    }
}
