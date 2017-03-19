using System;
using System.Collections.Generic;
using Tact.Practices.LifetimeManagers;

namespace Tact.Practices
{
    public interface IContainer : IResolver
    {
        new IContainer BeginScope();
        
        void Register(ILifetimeManager lifetimeManager, Type type, string key = null);

        object Resolve(Stack<Type> stack, Type type, string key = null);
        bool TryResolve(out object result, Stack<Type> stack, Type type, string key = null);

        bool CanResolve(Stack<Type> stack, Type type, string key = null);
        bool CanCreateInstance(Stack<Type> stack, Type type);

        IEnumerable<object> ResolveAll(Stack<Type> stack, Type type);

        object CreateInstance(Stack<Type> stack, Type type);
        bool TryCreateInstance(out object result, Stack<Type> stack, Type type);

        bool TryResolveGenericType(out object result, Stack<Type> stack, Type genericType, Type[] genericArguments, string key = null);
    }
}