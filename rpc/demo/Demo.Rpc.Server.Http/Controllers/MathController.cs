using Demo.Rpc.Models;
using Demo.Rpc.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tact;
using Tact.Practices;

namespace Demo.Rpc.Controllers
{
    [Route("rpc/[controller]")]
    public class MathController : Controller
    {
        private readonly IMathService _mathService;

        public MathController(IResolver resolver)
        {
            _mathService = resolver.Resolve<IMathService>();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Sum([FromBody]SumRequest sumRequest)
        {
            var result = await _mathService.SumAsync(sumRequest);
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
