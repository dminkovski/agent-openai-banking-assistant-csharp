
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{userName}/accounts")]
    public ActionResult<List<Account>> GetAccountsByUserName(string userName)
    {
        _logger.LogInformation("Received request to get accounts for user: {UserName}", userName);
        return Ok(_userService.GetAccountsByUserName(userName));
    }
}

