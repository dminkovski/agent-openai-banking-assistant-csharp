using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors;
using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Microsoft.SemanticKernel.Agents.Chat;
using AgentBankingAssistant.Copilot.Interfaces;
using System.Runtime.ConstrainedExecution;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.OpenApi.Services;
using Microsoft.SemanticKernel.Agents;

namespace AgentBankingAssistant.Copilot.Services.Agents
{
    public abstract class AgentService
    {
        private string systemPrompt;

        private readonly string modelId;

        private readonly ILogger<AgentService> _logger;

        private Kernel kernel;

        private ChatHistory chatHistory;

        private AzureOpenAIClient client;

        protected readonly AzureOpenAIChatCompletionService chat;

        public static event EventHandler FunctionCalled;

        protected virtual void OnFunctionCalled(EventArgs e)
        {
            FunctionCalled?.Invoke(this, e);
        }

        //protected readonly IToolsExecutionCache _toolsExecutionCache;
        public Stream? GetAPIYaml(string apiName)
        {
            try
            {
                // Get the current assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Define the full resource name (namespace + file name)
                var resourceNames = assembly.GetManifestResourceNames();

                // Find the resource that matches the apiName parameter
                var resourceName = resourceNames
                    .FirstOrDefault(name => name.EndsWith(string.Concat(apiName, ".yaml"), StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    // If no resource is found, throw an exception
                    throw new InvalidOperationException($"Resource '{apiName}.yaml' not found.");
                }

                // Return the stream for the found resource
                return assembly.GetManifestResourceStream(resourceName);
            }
            catch (Exception ex)
            {
                // Handle any error (e.g., file not found, IO issues, etc.)
                throw new InvalidOperationException("Error reading the embedded YAML file.", ex);
            }
        }


        public AgentService(AzureOpenAIClient client, string modelId, string apiUrl, string apiName, ILogger<AgentService> logger) {

            this.client = client;

            _logger = logger;

            //TODO: LoggedUserService, ToolExecutionCache

            IKernelBuilder builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(""/*question*/, client, modelId);

            kernel = builder.Build();

            //Kernel Plugin function for api

            var pluginName = char.ToUpper(apiName[0]) + apiName.Substring(1) + "Plugin";

            AddOpenAPIPlugin(apiName, pluginName, apiUrl);

            // TODO: Add to cache

            //TODO: Add hook

        }
        public async void AddOpenAPIPlugin(string apiName, string pluginName, string apiUrl)
        {
            var stream = GetAPIYaml(apiName);

            var uri = new Uri(apiUrl);

            #pragma warning disable SKEXP0040 // Experimental API 
            OpenApiFunctionExecutionParameters parameters = new OpenApiFunctionExecutionParameters(serverUrlOverride: uri);

            KernelPlugin plugin = await OpenApiKernelPluginFactory.CreateFromOpenApiAsync(pluginName, stream!, parameters);

            kernel.Plugins.Add(plugin);
        }
        public virtual void CreateAgent(ChatHistory userChatHistory,AgentContext agentContext, string agentName) {
            _logger.LogInformation("======== Agent: Starting ========");

            // Create extended system message

            string extendedSystemMessage = "";

            ChatCompletionAgent agent = new()
            {
                Name = agentName,
                Kernel = kernel,
                Instructions = extendedSystemMessage

            };

            // TODO: Get User Context

            // TODO: Get Cache

            // Get chat history

            var agentChatHistory = new ChatHistory(extendedSystemMessage);

            foreach (var chatMessage in userChatHistory)
            {
                if (chatMessage.Role != AuthorRole.System && chatMessage.Content != null)
                {
                    agentChatHistory.AddMessage(chatMessage.Role, chatMessage.Content);
                }
            }

            var executionSettings = new AzureOpenAIPromptExecutionSettings
            {
                Temperature = 0.1,
                TopP = 1,
                PresencePenalty = 0,  
                FrequencyPenalty = 0
            };


            // Get messages
            var messages = this.chat.GetChatMessageContentsAsync(
                                                                    agentChatHistory,
                                                                    executionSettings: executionSettings,
                                                                    kernel);

            // Get last message
            var message = messages.GetAwaiter().GetResult().Last();

            // Logging 

            _logger.LogInformation("======== Account Agent Response: {}", message.Content);

            // Set Agent context

            agentContext.setResult(message.Content);
        }

    }
}
