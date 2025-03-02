using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopHub.API.Helper;

namespace ShopHub.API.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Error(int statusCode)
        {
            return new ObjectResult(new ResponseAPI(statusCode));
        }
    }
}
