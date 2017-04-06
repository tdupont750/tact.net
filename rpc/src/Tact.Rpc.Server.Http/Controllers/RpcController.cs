using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Rpc.Practices;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Controllers
{
    [Route("rpc/{service}")]
    public class RpcController : Controller
    {
        private readonly IResolver _resolver;
        private readonly IReadOnlyList<RpcServiceInfo> _rpcServices;
        private readonly IReadOnlyList<ISerializer> _serializers;

        public RpcController(IResolver resolver)
        {
            _resolver = resolver;
            _rpcServices = resolver.Resolve<IReadOnlyList<RpcServiceInfo>>();
            _serializers = resolver.Resolve<IReadOnlyList<ISerializer>>();
        }

        [HttpPost("{method}")]
        public async Task<IActionResult> Invoke(string service, string method)
        {
            var serializer = _serializers.FirstOrDefault(s => HttpContext.Request.ContentType.StartsWith(s.ContentType, StringComparison.OrdinalIgnoreCase));
            if (serializer == null)
                BadRequest($"Invalid Content Type: {HttpContext.Request.ContentType}");

            foreach (var rpcService in _rpcServices)
                if (rpcService.CanHandle(service, method, out Type type))
                {
                    object model;
                    try
                    {
                        model = await serializer
                            .DeserializeAsync(type, HttpContext.Request.Body)
                            .ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        return BadRequest($"Unable To Deserialize: {type.Name}");
                    }
                    
                    var result = await rpcService
                        .HandleAsync(_resolver, method, model)
                        .ConfigureAwait(false);

                    return Ok(result);
                }

            return NotFound();
        }
    }
}
