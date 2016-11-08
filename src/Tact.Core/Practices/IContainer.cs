using System;
using System.Collections.Generic;
using Tact.Practices.LifetimeManagers;

namespace Tact.Practices
{
    public interface IContainer : IResolver
    {
        void Register(Type type, ILifetimeManager lifetimeManager);
        void Register(Type type, string key, ILifetimeManager lifetimeManager);

        object CreateInstance(Type type, Stack<Type> stack);

        object Resolve(Type type, Stack<Type> stack);
        object Resolve(Type type, string name, Stack<Type> stack);

        bool TryResolve(Type type, Stack<Type> stack, out object result);
        bool TryResolve(Type type, string name, Stack<Type> stack, out object result);

        IEnumerable<object> ResolveAll(Type type, Stack<Type> stack);
    }
}