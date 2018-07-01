using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

[Route("api/[controller]")]
public class XsrfTokenController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;
    public XsrfTokenController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }   


    public IActionResult Get()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return new ObjectResult(new {
            token = tokens.RequestToken,
            tokenName = tokens.HeaderName
        });
    }
}