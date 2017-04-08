using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Reflection;

namespace Tact.Rpc.Services.Implementation
{
    public class RpcHandler : IRpcHandler
    {
        private readonly Dictionary<string, Tuple<Lazy<EfficientInvoker>, Type>> _methods;

        private readonly Type _type;

        public RpcHandler(Type type)
        {
            _type = type;

            Name = type.Name.GetRpcName();

            _methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(
                    k => k.Name.GetRpcName(),
                    v => 
                    {
                        var invoker = new Lazy<EfficientInvoker>(() => type.GetMethodInvoker(v.Name));
                        var firstArg = v.GetParameterTypes().Single();
                        return Tuple.Create(invoker, firstArg);
                    }, 
                    StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }

        public bool CanHandle(string service, string method, out Type argType)
        {
            if (Name.Equals(service, StringComparison.OrdinalIgnoreCase)
                && _methods.TryGetValue(method, out Tuple<Lazy<EfficientInvoker>, Type> methodTuple))
            {
                argType = methodTuple.Item2;
                return true;
            }

            argType = null;
            return false;
        }

        public Task<object> HandleAsync(IResolver resolver, string method, object model)
        {
            var service = resolver.Resolve(_type);
            var invoker = _methods[method].Item1.Value;
            return invoker.InvokeAsync(service, model);
        }
    }
}
