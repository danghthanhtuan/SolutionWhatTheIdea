using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore
{
    public static class ConsumeHelper
    {
        public static void ExecutionAction(string function, ILogger logger, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.LogError("ConsumeHelper ExecutionAction {0} => {1}", function, ex);
                throw;
            }
        }

        public static async Task ExecutionActionAsync(string function, ILogger logger, Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                logger.LogError("ConsumeHelper ExecutionAction {0} => {1}", function, ex);
                throw;
            }
        }
    }
}