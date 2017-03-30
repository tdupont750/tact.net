using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tact.Rpc.Serialization
{
    public interface ISerializer
    {
        string ContentType { get; }
        Encoding Encoding { get; }

        object Deserialize(Type type, string value, bool isEnvelope = false);
        object Deserialize(Type type, byte[] value, bool isEnvelope = false);
        Task<object> DeserializeAsync(Type type, Stream stream, bool isEnvelope = false);

        string SerializeToString(object obj, bool useEnvelope = false);
        byte[] SerializeToBytes(object obj, bool useEnvelope = false);
        Task SerializeToStreamAsync(object obj, Stream stream, bool useEnvelope = false);

    }
}
