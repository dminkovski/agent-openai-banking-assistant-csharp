using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;


public class AccountAgent
{
    public ChatCompletionAgent agent;
    private readonly ILogger<AccountAgent> _logger;


    public AccountAgent(Kernel kernel, IConfiguration configuration, IUserService userService, ILogger<AccountAgent> logger)
    {
        _logger = logger;
        Kernel toolKernel = kernel.Clone();

        var accountsApiURL = configuration["BackendAPIs:AccountsApiUrl"];

        AgenticUtils.AddOpenAPIPlugin( 
           kernel: toolKernel,
           pluginName: "AccountsPlugin",
           apiName: "account",
           apiUrl: accountsApiURL
        );

        agent =
        new()
        {
            Name = "AccountAgent",
            Instructions = String.Format(AgentInstructions.AccountAgentInstructions, userService.GetLoggedUser()),
            Kernel = toolKernel,
            Arguments =
            new KernelArguments(
                new AzureOpenAIPromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }
            )
        };
    }
}
