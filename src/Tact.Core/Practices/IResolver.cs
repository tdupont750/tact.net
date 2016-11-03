using System;
using System.Collections.Generic;

namespace Tact.Practices
{
    public interface IResolver : IDisposable
    {
        IResolver BeginScope();

        object Resolve(Type type);
        object Resolve(Type type, string key);

        IEnumerable<object> ResolveAll(Type type);
    }
}