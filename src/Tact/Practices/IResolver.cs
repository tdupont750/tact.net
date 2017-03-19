using System;
using System.Collections.Generic;
using Tact.Threading;

namespace Tact.Practices
{
    public interface IResolver : IAsyncDisposable
    {
        IResolver BeginScope();
        
        object Resolve(Type type, string key = null);

        bool TryResolve(out object result, Type type, string key = null);

        IEnumerable<object> ResolveAll(Type type);
    }
}