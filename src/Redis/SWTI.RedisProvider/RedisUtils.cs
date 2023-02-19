using System.Reflection;
using StackExchange.Redis;

namespace SWTI.RedisProvider
{
    public static class RedisUtils
    {
        public static HashEntry[] ToHashEntries<T>(this T obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
             .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
             .Select(property =>
             {
                 if (property.PropertyType.IsClass)
                 {
                     return new HashEntry(property.Name, MessagePack.MessagePackSerializer.Serialize(property.GetValue(obj)));
                 }
                 return new HashEntry(property.Name, property.GetValue(obj).ToString());
             }

             ).ToArray();
        }

        //Deserialize from Redis format
        public static T ConvertHashToOjbect<T>(this HashEntry[] hashEntries)
        {
            if (hashEntries.Length <= 0)
            {
                return default;
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                //if (entry.Value.StartsWith("[") || entry.Value.StartsWith("{"))
                if (property.PropertyType.IsClass)
                {
                    property.SetValue(obj, Convert.ChangeType(MessagePack.MessagePackSerializer.Deserialize(property.PropertyType, entry.Value), property.PropertyType));
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
                }
            }
            return (T)obj;
        }
    }
}
