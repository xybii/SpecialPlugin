using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpecialPlugin.Project.OldDapperDemo;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.ReplaceController.Controllers
{
    [Route("New")]
    public class NewController : ControllerBase
    {
        private readonly ITest _test;

        public NewController(ITest test)
        {
            _test = test;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(Order = 1)]
        public async Task<IActionResult> Get()
        {
            return Ok(_test.Get());
        }
    }
}
