using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
//using Tact.Rpc.Practices;

namespace Tact.Rpc.Generator
{
    public class ClientClassTemplate
    {
        public static ClientClassTemplate Create(SyntaxTree tree, GeneratorConfig config)
        {
            var syntaxRoot = tree.GetRoot();

            var namespaceNode = syntaxRoot
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Single();

            var interfaceNode = namespaceNode
                .DescendantNodes()
                .OfType<InterfaceDeclarationSyntax>()
                .Single();

            var isServiceDefinition = interfaceNode.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => "RpcServiceDefinitionAttribute".StartsWith(a.Name.ToString()));

            if (!isServiceDefinition)
                return null;

            var serviceNamespace = namespaceNode.Name.ToString();

            var usingStrings = syntaxRoot
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(u => u.Name.ToString())
                .Concat(config.IncludeNamespaces.Split('|'))
                .Concat(new [] { serviceNamespace })
                .Distinct()
                .OrderBy(s => s)
                .Select(s => $"using {s};")
                .ToArray();

            var methods = interfaceNode
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(m => ClientMethodTemplate.Create(m))
                .ToList();

            return new ClientClassTemplate(methods, config)
            {
                RootNamespace = serviceNamespace.Replace("." + config.ServiceNamespaceSuffix, string.Empty),
                InterfaceName = interfaceNode.Identifier.Text,
                UsingStatements = string.Join(Environment.NewLine, usingStrings)
            };
        }

        private readonly List<ClientMethodTemplate> _methods;

        private readonly GeneratorConfig _config;

        public ClientClassTemplate(List<ClientMethodTemplate> methods, GeneratorConfig config)
        {
            _methods = methods;
            _config = config;
        }

        public string UsingStatements { get; set; }

        public string RootNamespace { get; set; }

        public string InterfaceName { get; set; }

        public string ClassName => ServiceName + _config.ClientClassSuffix;

        public string ServiceName => InterfaceName.GetRpcName();

        public string Methods => string.Join(Environment.NewLine, _methods).Trim();

        public void AddMethod(ClientMethodTemplate method)
        {
            _methods.Add(method);
        }

        public override string ToString()
        {
            return $@"
{UsingStatements}

namespace {RootNamespace}.Clients.Implementation
{{
    [RpcClientImplementation(typeof({InterfaceName}))]
    public class {ClassName} : {InterfaceName}
    {{
        private const string ServiceName = ""{ServiceName}"";

        private readonly IRpcClient _rpcClient;
        
        public {ServiceName}Client(IRpcClientManager clientManager) =>
            _rpcClient = clientManager.GetRpcClient(ServiceName);

        {Methods}
    }}
}}".Trim();
        }

    }
}