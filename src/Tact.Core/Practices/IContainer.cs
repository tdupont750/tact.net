using System;
using System.Collections.Generic;
using Tact.Practices.Registration;

namespace Tact.Practices
{
    public interface IContainer : IResolver
    {
        void Register(Type type, IRegistration registration);
        void Register(Type type, string key, IRegistration registration);

        object CreateInstance(Type type, Stack<Type> stack);

        object Resolve(Type serviceType, Stack<Type> stack);
        object Resolve(Type serviceType, string name, Stack<Type> stack);

        IEnumerable<object> ResolveAll(Type serviceType, Stack<Type> stack);
    }
}