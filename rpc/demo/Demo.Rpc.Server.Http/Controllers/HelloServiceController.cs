using Demo.Rpc.Models;
using Demo.Rpc.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tact;
using Tact.Practices;

namespace Demo.Rpc.Controllers
{
    [Route("rpc/[controller]")]
    public class HelloServiceController : Controller
    {
        private readonly IHelloService _helloService;

        public HelloServiceController(IResolver resolver)
        {
            _helloService = resolver.Resolve<IHelloService>();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SayHello([FromBody]HelloRequest helloRequest)
        {
            var result = await _helloService.SayHelloAsync(helloRequest);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult Test()
        {
            // TODO RETURN MANIFEST
            return Ok(true);
        }
    }
}
