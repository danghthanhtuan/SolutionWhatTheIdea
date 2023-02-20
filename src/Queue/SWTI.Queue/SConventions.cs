using EasyNetQ;

namespace SWTI.QueueCore
{
    public class SConventions : Conventions
    {
        public SConventions(ITypeNameSerializer typeNameSerializer)
            : base(typeNameSerializer)
        {
            base.QueueNamingConvention = delegate (Type type, string subscriptionId)
            {
                if (Storage.QueueNameStore == null)
                {
                    Storage.QueueNameStore = new List<string>();
                }

                string convention = typeNameSerializer.Serialize(type);
                if (!Storage.QueueNameStore.Any((string x) => x.Equals(convention)))
                {
                    Storage.QueueNameStore.Add(convention);
                }

                return convention;
            };
            base.ExchangeNamingConvention = (Type type) => typeNameSerializer.Serialize(type);
        }
    }
}
