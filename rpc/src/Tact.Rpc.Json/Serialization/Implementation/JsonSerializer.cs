using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tact.Practices.LifetimeManagers.Attributes;

namespace Tact.Rpc.Serialization.Implementation
{
    [RegisterSingleton(typeof(ISerializer), "JSON")]
    public class JsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer = new Newtonsoft.Json.JsonSerializer();

        public string ContentType => "application/json";

        public Encoding Encoding => Encoding.UTF8;
        
        public object Deserialize(Type type, string value)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        public object Deserialize(Type type, byte[] value)
        {
            var json = Encoding.GetString(value);
            return Deserialize(type, json);
        }

        public Task<object> DeserializeAsync(Type type, Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding, true, 1024, true))
            using (var jr = new JsonTextReader(sr))
            {
                var result = _jsonSerializer.Deserialize(jr, type);
                
                // https://github.com/JamesNK/Newtonsoft.Json/issues/803
                if (stream is MemoryStream memoryStream)
                    memoryStream.Position = jr.LinePosition;

                return Task.FromResult(result);
            }
        }

        public string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public byte[] SerializeToBytes(object obj)
        {
            var json = SerializeToString(obj);
            return Encoding.GetBytes(json);
        }

        public async Task SerializeToStreamAsync(object obj, Stream stream)
        {
            var json = SerializeToString(obj);
            using (var streamWriter = new StreamWriter(stream))
                await streamWriter
                    .WriteAsync(json)
                    .ConfigureAwait(false);
        }
    }
}
