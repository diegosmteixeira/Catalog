using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{v:apiVersion}/test")]
    [ApiController]
    public class TestV2Controller : Controller
    {
        public IActionResult Get()
        {
            return Content("<html><body><h2>TestV2Controller - V 2.0 </h2></body></html>", "text/html");
        }
    }
}
