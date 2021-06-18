using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.ReplaceController.Controllers
{
    [Route("New")]
    public class NewController : ControllerBase
    {
        [HttpGet(Order = 1)]
        public async Task<IActionResult> Get()
        {
            return Ok("ReplaceController");
        }
    }
}
