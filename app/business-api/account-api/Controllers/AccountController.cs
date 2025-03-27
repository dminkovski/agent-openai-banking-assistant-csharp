[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet("{accountId}")]
    public ActionResult<Account> GetAccountDetails(string accountId)
    {
        _logger.LogInformation("Received request to get account details for account id: {AccountId}", accountId);
        return Ok(_accountService.GetAccountDetails(accountId));
    }

    [HttpGet("{accountId}/paymentmethods/{methodId}")]
    public ActionResult<PaymentMethod> GetPaymentMethodDetails(string accountId, string methodId)
    {
        _logger.LogInformation("Received request to get payment method details for account id: {AccountId} and method id: {MethodId}",
            accountId, methodId);
        return Ok(_accountService.GetPaymentMethodDetails(methodId));
    }

    [HttpGet("{accountId}/registeredBeneficiaries")]
    public ActionResult<List<Beneficiary>> GetBeneficiaryDetails(string accountId)
    {
        _logger.LogInformation("Received request to get beneficiary details for account id: {AccountId}", accountId);
        return Ok(_accountService.GetRegisteredBeneficiary(accountId));
    }
}