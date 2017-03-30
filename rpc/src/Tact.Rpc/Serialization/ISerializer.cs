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

        object Deserialize(Type type, string value);
        object Deserialize(Type type, byte[] value);
        Task<object> DeserializeAsync(Type type, Stream stream);

        string SerializeToString(object obj);
        byte[] SerializeToBytes(object obj);
        Task SerializeToStreamAsync(object obj, Stream stream);
    }
}