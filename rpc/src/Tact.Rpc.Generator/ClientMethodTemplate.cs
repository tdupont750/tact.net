using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tact.Rpc.Generator
{
    public class ClientMethodTemplate
    {
        private static readonly Regex ResponseTypeRegex = new Regex("^Task<(.+)>$", RegexOptions.Compiled);

        public static ClientMethodTemplate Create(MethodDeclarationSyntax method)
        {
            var request = method.ParameterList.Parameters.Single().Type.ToString();
            var match = ResponseTypeRegex.Match(method.ReturnType.ToString());

            if (!match.Success)
                throw new InvalidOperationException("All methods must return a Task");

            return new ClientMethodTemplate
            {
                RequestType = request,
                ClassMethodName = method.Identifier.Text,
                ResponseType = match.Groups[1].Value
            };
        }

        private ClientMethodTemplate()
        {
        }

        public string RequestType { get; set; }

        public string ResponseType { get; set; }

        public string ClassMethodName { get; set; }

        public string ServiceMethodName => ClassMethodName.GetRpcName();

        public override string ToString()
        {
            return $@"
        public Task<{ResponseType}> {ClassMethodName}({RequestType} model) =>
            _rpcClient.SendAsync<{RequestType}, {ResponseType}>(
                ServiceName,
                ""{ServiceMethodName}"",
                model);";
        }
    }
}