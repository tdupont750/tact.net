using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Practices;
using Tact.Rpc.Serialization;
using Demo.Rpc.Services.Base;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterHttpClientCondition(ServiceName), RegisterSingleton(typeof(IMathService))]
    public class MathServiceHttpClient : HttpClientBase, IMathService
    {
        private const string ServiceName = "MathService";

        public MathServiceHttpClient(IResolver resolver)
            :base(resolver, ServiceName)
        {
        }

        private MathServiceHttpClient(string hostUrl, ISerializer serializer)
            : base(ServiceName, hostUrl, serializer)
        {
        }

        public MathServiceHttpClient Create(string hostUrl, ISerializer serializer)
        {
            return new MathServiceHttpClient(hostUrl, serializer);
        }

        public Task<SumResponse> SumAsync(SumRequest sumRequest)
        {
            return SendAsync<SumRequest, SumResponse>(sumRequest, "Sum");
        }
    }
}
