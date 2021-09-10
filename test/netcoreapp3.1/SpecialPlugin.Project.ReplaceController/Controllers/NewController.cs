using Microsoft.AspNetCore.Mvc;

namespace SpecialPlugin.Project.ReplaceController.Controllers
{
    [Route("New")]
    public class NewController : ControllerBase
    {
        [HttpGet(Order = 1)]
        public IActionResult Get()
        {
            return Ok("ReplaceController");
        }
    }
}
