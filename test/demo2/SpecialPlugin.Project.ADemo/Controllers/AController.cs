using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpecialPlugin.Project.ADemo;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.NewDapperDemo.Controllers
{
    [Authorize]
    [Route("New")]
    public class AController : ControllerBase
    {
        private readonly ITest _test;

        public AController(ITest test)
        {
            _test = test;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("2", Order = 1)]
        public async Task<IActionResult> Get2()
        {
            return Ok(_test.Get());
        }
    }
}
