using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    //Deprecated = shows in the header that v1 is obsolete
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/test")]
    [ApiController]
    public class TestV1Controller : Controller
    {
        public IActionResult Get()
        {
            return Content("<html><body><h2>TestV1Controller - V 1.0 </h2></body></html>", "text/html");
        }
    }
}