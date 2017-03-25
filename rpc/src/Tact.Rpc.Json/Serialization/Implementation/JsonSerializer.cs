using System.Text;
using Newtonsoft.Json;
using Tact.Practices.LifetimeManagers.Attributes;

namespace Tact.Rpc.Serialization.Implementation
{
    [RegisterSingleton(typeof(ISerializer))]
    public class JsonSerializer : ISerializer
    {
        public string MediaType => "application/json";

        public Encoding Encoding => Encoding.UTF8;

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
