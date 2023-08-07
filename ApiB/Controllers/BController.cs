using Microsoft.AspNetCore.Mvc;

namespace ApiB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BController : ControllerBase
    {
        public BController()
        {

        }

        [HttpGet()]
        public ActionResult<string> Get()
        {
            return Ok(@"Hello, World!");
        }
    }
}