using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Rpc.Serialization;
using Tact.Rpc.Services;

namespace Tact.Rpc.Controllers
{
    [Route("rpc/{service}")]
    public class RpcController : Controller
    {
        private readonly IResolver _resolver;
        private readonly IReadOnlyList<IRpcHandler> _rpcHandlers;
        private readonly IReadOnlyList<ISerializer> _serializers;

        public RpcController(IResolver resolver)
        {
            _resolver = resolver;
            _rpcHandlers = resolver.Resolve<IReadOnlyList<IRpcHandler>>();
            _serializers = resolver.Resolve<IReadOnlyList<ISerializer>>();
        }

        [HttpPost("{method}")]
        public async Task<IActionResult> Invoke(string service, string method)
        {
            var serializer = _serializers.FirstOrDefault(s => HttpContext.Request.ContentType.StartsWith(s.ContentType, StringComparison.OrdinalIgnoreCase));
            if (serializer == null)
                BadRequest($"Invalid Content Type: {HttpContext.Request.ContentType}");

            foreach (var rpcService in _rpcHandlers)
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
