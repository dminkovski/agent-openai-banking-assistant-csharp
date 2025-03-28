
using Microsoft.AspNetCore.Authorization;

[Route("/api/auth_setup")]
[ApiController]
public class AuthSetupController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { UseLogin = false });
    }
}
