using Microsoft.AspNetCore.Mvc;
using SpecialPlugin.Project.ADemo;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.NewDapperDemo.Controllers
{
    [Route("New")]
    public class NewController : ControllerBase
    {
        private readonly ITest _test;

        public NewController(ITest test)
        {
            _test = test;
        }

        [HttpGet("2", Order = 1)]
        public async Task<IActionResult> Get2()
        {
            return Ok(_test.Get());
        }
    }
}
