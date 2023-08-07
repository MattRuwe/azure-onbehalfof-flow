using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;

namespace ApiA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AController : ControllerBase
    {
        private IDownstreamApi _downstreamApi;

        public AController(IDownstreamApi downstreamApi)
        {
            _downstreamApi = downstreamApi;
        }

        [HttpGet()]
        public async Task<ActionResult<string>> Get()
        {
            var response = await _downstreamApi.CallApiForUserAsync("ApiB", options =>
            {
                options.RelativePath = "B";
            });
            var responseCode = response.StatusCode;
            var responseString = await response.Content.ReadAsStringAsync();
            return Ok(responseString);
        }
    }
}