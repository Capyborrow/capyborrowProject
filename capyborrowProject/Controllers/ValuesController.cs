using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For testing. Will be replaced or deleted later

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ValuesController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Hello, world!");
        }
    }
}
