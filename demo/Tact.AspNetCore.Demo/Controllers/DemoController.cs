using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tact.AspNetCore.Demo.Services;

namespace Tact.AspNetCore.Demo
{
    [Route("api/[controller]")]
    public class DemoController: Controller
    {
        private readonly IHelloService _helloService;

        public DemoController(IHelloService helloService)
        {
            _helloService = helloService;
        }

        [HttpGet("[action]/{name}")]
        public IActionResult Hello(string name)
        {
            var result = _helloService.SayHello(name);
            return Ok(result);
        }
    }
}
