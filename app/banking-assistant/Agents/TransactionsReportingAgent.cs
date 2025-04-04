﻿
public class TransactionsReportingAgent
{
    public ChatCompletionAgent agent;
    private ILogger _logger;
    public TransactionsReportingAgent(Kernel kernel, IConfiguration configuration, IUserService userService, ILogger<TransactionsReportingAgent> logger)
    {
        _logger = logger;
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

        this.agent =
        new()
        {
            Name = "TransactionsReportingAgent",
            Instructions = String.Format(AgentInstructions.TransactionsReportingAgentInstructions, userService.GetLoggedUser()),
            Kernel = toolKernel,
            Arguments =
            new KernelArguments(
                new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            )
        };
    }
}

