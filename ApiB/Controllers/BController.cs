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
            //Debug to checkout claims available
            var name = $"{User?.Claims.SingleOrDefault(_ => _.Type == "name")?.Value} ({User?.Identity?.Name})";
            return Ok($"Hello, {name ?? "world"}!");
        }
    }
}