using Tact.Practices;

namespace Tact.Configuration
{
    public interface IInitializeAttribute
    {
        void Initialize(IContainer container);
    }
}