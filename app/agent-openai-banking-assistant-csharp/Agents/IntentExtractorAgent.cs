﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.Reflection.Metadata;
using System.Text.Json;

namespace agent_openai_banking_assistant_csharp.Agents
{
    public class IntentExtractorAgent
    {

        private IChatCompletionService _chatCompletionService;

        private Kernel _kernel;


        private string INTENT_SYSTEM_MESSAGE = $$$"""
                You are a personal financial advisor who help bank customers manage their banking accounts and services.
                The user may need help with his recurrent bill payments, it may start the payment checking payments history for a specific payee.
                In other cases it may want to just review account details or transactions history.
                Based on the conversation you need to identify the user intent.
                The available intents are:
                "BillPayment","RepeatTransaction","TransactionHistory","AccountInfo"
                If none of the intents are identified provide the user with the list of the available intents.

                If an intent is identified return the output as json format as below
                {
                "intent": "BillPayment"
                 }

                If you don't understand or if an intent is not identified be polite with the user, ask clarifying question also using the list of the available intents. 
                Don't add any comments in the output or other characters, just use json format.
            
                """;

        public IntentExtractorAgent(IConfiguration configuration) 
        {
            var deploymentName = configuration[key: "AzureOpenAPI:Deployment"];
            var endpoint = configuration[key: "AzureOpenAPI:Endpoint"];
            var apiKey = configuration[key: "AzureOpenAPI:ApiKey"];


            Console.WriteLine($"Instantiating AzureOpenAIChatCompletion: {deploymentName} {endpoint}");

            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName: deploymentName, endpoint:endpoint, apiKey:apiKey);
            Kernel kernel = kernelBuilder.Build();
            this._chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            this._kernel = kernel;
        }

        public async Task<IntentResponse> Run(ChatHistory userChatHistory)
        {
            var agentchatHistory = new ChatHistory();
            agentchatHistory.AddRange(fewShotExamples());
            agentchatHistory.AddRange(userChatHistory);

            var peSettings = new PromptExecutionSettings();
            ChatMessageContent results = await this._chatCompletionService.GetChatMessageContentAsync(chatHistory: agentchatHistory, kernel: this._kernel);
            var content = results.Content;
            JsonDocument jsonData;

            /**
            * Try to see if the model answered with a formatted json. If not it is just trying to keep the conversation going to understand the user intent
            * but without answering with a formatted output. In this case the intent is None and the clarifying sentence is not used.
            */
            try
            {
                jsonData = JsonDocument.Parse(content);

            }
            catch (JsonException ex)
            {
                Console.Write(ex.ToString());
                return new IntentResponse(IntentType.None, content);
            }

            JsonElement root = jsonData.RootElement;

            string intentString = root.GetProperty("intent").GetString();
            IntentType intentType = (IntentType)Enum.Parse(typeof(IntentType), intentString);
            string clarifySentence = "";
            try
            {
               clarifySentence = root.GetProperty("clarify_sentence").GetString();
            }
            catch (Exception ex)
            {
                // this is the case where the intent has been identified and the clarifying sentence is not present in the json output
            }

            return new IntentResponse(intentType, clarifySentence.ToString() ?? "");
        }

        ChatHistory fewShotExamples()
        {
            var examplesHistory = new ChatHistory();

            examplesHistory.AddUserMessage("can you buy stocks for me?");
            examplesHistory.AddAssistantMessage("{\"intent\": \"None\", \"clarify_sentence\":\"I'm sorry can't help with that.I can review your account details, transactions and help you with your payments\"");
            examplesHistory.AddUserMessage("can you pay this bill for me?");
            examplesHistory.AddAssistantMessage("{\"intent\": \"BillPayment\" }");
            examplesHistory.AddUserMessage("when was last time I paid acme");
            examplesHistory.AddAssistantMessage("{\"intent\": \"TransactionHistory\" }");
            examplesHistory.AddUserMessage("proceed with payment");
            examplesHistory.AddAssistantMessage("{\"intent\": \"BillPayment\" }");

            return examplesHistory;
        }
    }
}
