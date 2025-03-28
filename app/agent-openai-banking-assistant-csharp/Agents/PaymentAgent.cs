
using Microsoft.Extensions.Logging;

public class PaymentAgent
{
    public ChatCompletionAgent agent;
    private ILogger<PaymentAgent> _logger;
    public PaymentAgent(Kernel kernel, IConfiguration configuration, IDocumentScanner documentScanner, IUserService userService, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PaymentAgent>();

        Kernel toolKernel = kernel.Clone();

        var transactionApiURL = configuration["BackendAPIs:TransactionsApiUrl"];
        var accountsApiURL = configuration["BackendAPIs:AccountsApiUrl"];
        var paymentsApiURL = configuration["BackendAPIs:PaymentsApiUrl"];

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "TransactionHistoryPlugin",
           apiName: "transaction-history",
           apiUrl: transactionApiURL
        );

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "AccountsPlugin",
           apiName: "account",
           apiUrl: accountsApiURL
        );

        AgenticUtils.AddOpenAPIPlugin(
           kernel: toolKernel,
           pluginName: "PaymentsPlugin",
           apiName: "payments",
        apiUrl: paymentsApiURL
        );

        toolKernel.Plugins.AddFromObject(new InvoiceScanPlugin(documentScanner, loggerFactory.CreateLogger<InvoiceScanPlugin>()), "InvoiceScanPlugin");
       
        this.agent =
        new()
        {
            Name = "PaymentAgent",
            Instructions = String.Format(AgentInstructions.PaymentAgentInstructions, userService.GetLoggedUser()),
            Kernel = toolKernel,
            Arguments =
            new KernelArguments(
                new AzureOpenAIPromptExecutionSettings(){ FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            )
        };
    }
}
