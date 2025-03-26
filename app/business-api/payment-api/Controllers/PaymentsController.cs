[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IPaymentService _paymentService;

    public PaymentsController(
        ILogger<PaymentsController> logger,
        IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitPayment([FromBody] Payment payment)
    {
        _logger.LogInformation("Received payment request: {Payment}", payment);

        try
        {
            await _paymentService.ProcessPaymentAsync(payment);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogWarning(ex, "Invalid payment request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error processing payment");
            return StatusCode(500, "An error occurred while processing the payment");
        }
    }
}