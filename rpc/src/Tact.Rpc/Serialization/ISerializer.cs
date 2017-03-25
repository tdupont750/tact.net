using System.Text;

namespace Tact.Rpc.Serialization
{
    public interface ISerializer
    {
        string MediaType { get; }
        Encoding Encoding { get; }

        T Deserialize<T>(string value);
        string Serialize(object obj);
    }
}
