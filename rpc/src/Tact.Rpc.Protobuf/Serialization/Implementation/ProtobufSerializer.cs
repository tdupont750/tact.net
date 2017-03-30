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
        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly RuntimeTypeModel RuntimeTypeModel = TypeModel.Create();
        
        public string ContentType => "application/protobuf";

        public Encoding Encoding => Encoding.UTF8;

        public object Deserialize(Type type, string value)
        {
            var bytes = Encoding.GetBytes(value);
            return Deserialize(type, bytes);
        }

        public object Deserialize(Type type, byte[] value)
        {
            TryRegisterType(type);
            
            using (var memoryStream = new MemoryStream(value))
                return RuntimeTypeModel.DeserializeWithLengthPrefix(memoryStream, null, type, PrefixStyle.Base128, 0);
        }

        public Task<object> DeserializeAsync(Type type, Stream stream)
        {
            TryRegisterType(type);
            var result = RuntimeTypeModel.DeserializeWithLengthPrefix(stream, null, type, PrefixStyle.Base128, 0);
            return Task.FromResult(result);
        }

        public string SerializeToString(object obj)
        {
            var bytes = SerializeToBytes(obj);
            return Encoding.GetString(bytes);
        }

        public byte[] SerializeToBytes(object obj)
        {
            var type = obj.GetType();
            TryRegisterType(type);

            using (var stream = new MemoryStream())
            {
                RuntimeTypeModel.SerializeWithLengthPrefix(stream, obj, type, PrefixStyle.Base128, 0);
                return stream.ToArray();
            }
        }

        public Task SerializeToStreamAsync(object obj, Stream stream)
        {
            var type = obj.GetType();
            TryRegisterType(type);

            RuntimeTypeModel.SerializeWithLengthPrefix(stream, obj, type, PrefixStyle.Base128, 0);
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
    }
}
