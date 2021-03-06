using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Practices.LifetimeManagers
{
    public interface ILifetimeManager
    {
        string Description { get; }

        bool IsScoped { get; }

        bool IsDisposable { get; }

        ILifetimeManager CloneWithGenericArguments(Type[] genericArguments);

        ILifetimeManager BeginScope(IContainer scope);

        object Resolve(IContainer scope, Stack<Type> stack);

        bool RequiresDispose(IContainer scope);

        Task DisposeAsync(IContainer scope, CancellationToken cancelToken = default(CancellationToken));
    }
}
