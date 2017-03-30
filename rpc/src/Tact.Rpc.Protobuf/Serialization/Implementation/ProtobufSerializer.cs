using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Tact.Practices.LifetimeManagers.Attributes;
using System.Threading.Tasks;
using ProtoBuf;

namespace Tact.Rpc.Serialization.Implementation
{
    [RegisterSingleton(typeof(ISerializer), "Protobuf")]
    public class ProtobufSerializer : ISerializer
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type> { typeof(Envelope) };
        private static readonly RuntimeTypeModel RuntimeTypeModel = TypeModel.Create();
        
        public string ContentType => "application/protobuf";

        public Encoding Encoding => Encoding.UTF8;

        public object Deserialize(Type type, string value, bool isEnvelope = false)
        {
            var bytes = Encoding.GetBytes(value);
            return Deserialize(type, bytes, isEnvelope);
        }

        public object Deserialize(Type type, byte[] value, bool isEnvelope = false)
        {
            TryRegisterType(type);
            
            using (var memoryStream = new MemoryStream(value))
                return RuntimeTypeModel.Deserialize(memoryStream, null, type);
        }

        public Task<object> DeserializeAsync(Type type, Stream stream, bool isEnvelope = false)
        {
            TryRegisterType(type);
            var result = RuntimeTypeModel.Deserialize(stream, null, type);
            return Task.FromResult(result);
        }

        public string SerializeToString(object obj, bool useEnvelope = false)
        {
            var bytes = SerializeToBytes(obj, useEnvelope);
            return Encoding.GetString(bytes);
        }

        public byte[] SerializeToBytes(object obj, bool useEnvelope = false)
        {
            var type = obj.GetType();
            TryRegisterType(type);

            using (var stream = new MemoryStream())
            {
                RuntimeTypeModel.Serialize(stream, obj);
                var bytes = stream.ToArray();

                return useEnvelope
                    ? SerializeToBytes(new Envelope
                    {
                        Type = type.Name,
                        Content = bytes
                    })
                    : bytes;
            }
        }

        public Task SerializeToStreamAsync(object obj, Stream stream, bool useEnvelope = false)
        {
            var type = obj.GetType();
            TryRegisterType(type);

            if (useEnvelope)
            {
                var bytes = SerializeToBytes(obj);
                obj = new Envelope
                {
                    Type = type.Name,
                    Content = bytes
                };
            }

            RuntimeTypeModel.Serialize(stream, obj);
            return Task.CompletedTask;
        }
        
        // https://gist.github.com/devoyster/1473960
        // http://stackoverflow.com/questions/17201571/protobuf-net-serialization-without-attributes
        private static void TryRegisterType(Type type)
        {
            using (Lock.UseReadLock())
                if (RegisteredTypes.Contains(type))
                    return;

            using (Lock.UseWriteLock())
            {
                if (RegisteredTypes.Contains(type))
                    return;
                
                var metaType = RuntimeTypeModel.Add(type, false);
                metaType.AsReferenceDefault = true;
                metaType.UseConstructor = false;

                var serializableFields = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(fi => fi.Name)
                    .Select((fi, index) => new { info = fi, index });

                foreach (var field in serializableFields)
                {
                    var metaField = metaType.AddField(field.index + 1, field.info.Name);
                    metaField.AsReference = !field.info.PropertyType.GetTypeInfo().IsValueType;
                    metaField.DynamicType = field.info.PropertyType == typeof(object);
                }

                metaType.CompileInPlace();
                RegisteredTypes.Add(type);
            }
        }

        [ProtoContract]
        private class Envelope
        {
            [ProtoMember(1)]
            public string Type { get; set; }

            [ProtoMember(2)]
            public byte[] Content { get; set; }
        }
    }
}
