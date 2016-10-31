using System;
using System.Collections.Generic;

namespace Tact.Practices
{
    public interface IResolver : IDisposable
    {
        IResolver BeginScope();

        object Resolve(Type serviceType);
        object Resolve(Type serviceType, string key);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}