using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class XsrfTokenController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
 
        public XsrfTokenController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
    
            return new ObjectResult(new {
                token = tokens.RequestToken,
                tokenName = tokens.HeaderName
            });
        }
    }
}