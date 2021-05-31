using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SpecialPlugin.HttpApi.Controllers
{
    [Route("/")]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            return Ok("Get");
        }
    }
}
