using System.IO;
using System.Threading.Tasks;
using Tact.Rpc.Serialization;

namespace Tact.Rpc
{
    public static class SerializerExtensions
    {
        public static async Task<T> DeserializeAsync<T>(this ISerializer serializer, Stream stream)
        {
            return (T) await serializer
                .DeserializeAsync(typeof(T), stream)
                .ConfigureAwait(false);
        }
    
        public static T Deserialize<T>(this ISerializer serializer, string value)
        {
            return (T)serializer.Deserialize(typeof(T), value);
        }

        public static T Deserialize<T>(this ISerializer serializer, byte[] value)
        {
            return (T)serializer.Deserialize(typeof(T), value);
        }
    }
}
