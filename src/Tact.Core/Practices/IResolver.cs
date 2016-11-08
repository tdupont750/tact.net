using System;
using System.Collections.Generic;

namespace Tact.Practices
{
    public interface IResolver : IDisposable
    {
        IResolver BeginScope();

        object Resolve(Type type);
        object Resolve(Type type, string key);

        bool TryResolve(Type type, out object result);
        bool TryResolve(Type type, string key, out object result);

        IEnumerable<object> ResolveAll(Type type);
    }
}