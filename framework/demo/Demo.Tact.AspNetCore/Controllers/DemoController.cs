using Demo.Tact.AspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Tact.AspNetCore.Controllers
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
