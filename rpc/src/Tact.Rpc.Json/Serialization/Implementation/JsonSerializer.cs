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
        public string ContentType => "application/json";

        public Encoding Encoding => Encoding.UTF8;
        
        public object Deserialize(Type type, string value, bool isEnvelope = false)
        {
            if (isEnvelope)
            {
                var envelope = JsonConvert.DeserializeObject<Envelope>(value);
                return envelope.Content;
            }

            return JsonConvert.DeserializeObject(value, type);
        }

        public object Deserialize(Type type, byte[] value, bool isEnvelope = false)
        {
            var json = Encoding.GetString(value);
            return Deserialize(type, json, isEnvelope);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream, bool isEnvelope = false)
        {
            string json;

            using (var streamReader = new StreamReader(stream))
                json = await streamReader
                    .ReadToEndAsync()
                    .ConfigureAwait(false);

            return Deserialize(type, json, isEnvelope);
        }

        public string SerializeToString(object obj, bool useEnvelope = false)
        {
            if (useEnvelope)
                obj = new Envelope
                {
                    Type = obj.GetType().Name,
                    Content = obj
                };

            return JsonConvert.SerializeObject(obj);
        }

        public byte[] SerializeToBytes(object obj, bool useEnvelope = false)
        {
            var json = SerializeToString(obj, useEnvelope);
            return Encoding.GetBytes(json);
        }

        public async Task SerializeToStreamAsync(object obj, Stream stream, bool useEnvelope = false)
        {
            var json = SerializeToString(obj, useEnvelope);
            using (var streamWriter = new StreamWriter(stream))
                await streamWriter
                    .WriteAsync(json)
                    .ConfigureAwait(false);
        }

        private class Envelope
        {
            public string Type { get; set; }

            public object Content { get; set; }
        }
    }
}
