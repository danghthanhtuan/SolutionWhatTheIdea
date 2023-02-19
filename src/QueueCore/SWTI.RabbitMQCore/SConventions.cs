using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWTI.RabbitMQCore
{
    public class SConventions : Conventions
    {
        public SConventions(ITypeNameSerializer typeNameSerializer) : base(typeNameSerializer)
        {
            //ErrorQueueNamingConvention = messageInfo => "MyErrorQueue";
            QueueNamingConvention = (type, subscriptionId) =>
            {
                if (Storage.QueueNameStore == null)
                {
                    Storage.QueueNameStore = new List<string>();
                }
                var convention = typeNameSerializer.Serialize(type);
                if (Storage.QueueNameStore.Any(x => x.Equals(convention)) == false)
                    Storage.QueueNameStore.Add(convention);

                return convention;
            };
            ExchangeNamingConvention = type =>
            {
                return typeNameSerializer.Serialize(type);
            };
        }
    }
}
