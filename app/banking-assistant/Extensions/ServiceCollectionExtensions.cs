public static class ServicesExtensions
{
    public static IServiceCollection AddAzureServices(this IServiceCollection services)
    {

        // Register Azure Blob Service Client via the Azure Clients builder.
        services.AddSingleton<BlobServiceClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var credential = new DefaultAzureCredential();
            var accountName = configuration["Storage:AccountName"];
            var storageEndpoint = $"https://{accountName}.blob.core.windows.net";
            Console.WriteLine($"BlobServiceClient: {storageEndpoint}");
            var blobServiceClient = new BlobServiceClient(
                new Uri(storageEndpoint),
                credential);
            return blobServiceClient;
        });

        // Register BlobStorageProxy as IBlobStorage.
        services.AddSingleton<IBlobStorage>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var blobServiceClient = provider.GetRequiredService<BlobServiceClient>();
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<BlobStorageProxy>();
            return new BlobStorageProxy(blobServiceClient, logger, configuration);
        });

        // Register DocumentIntelligenceClient.
        services.AddSingleton<DocumentIntelligenceClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var endpoint = configuration["DocumentIntelligence:Endpoint"];
            var credential = new DefaultAzureCredential();
            return new DocumentIntelligenceClient(new Uri(endpoint), credential);
        });

        // Register DocumentIntelligenceProxy as IDocumentScanner.
        services.AddSingleton<IDocumentScanner, DocumentIntelligenceProxy>();

        // Register OpenAIClient.
        services.AddSingleton<AzureOpenAIClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var credential = new DefaultAzureCredential();
            return new AzureOpenAIClient(new Uri(endpoint), credential);
        });

        services.AddSingleton<Kernel>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var deploymentName = configuration[key: "AzureOpenAI:Deployment"];
            var endpoint = configuration[key: "AzureOpenAI:Endpoint"];
            var credential = new DefaultAzureCredential();

            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName: deploymentName, endpoint: endpoint, credentials: credential);
            return kernelBuilder.Build();
        });

        services.AddSingleton<IUserService, LoggedUserService>();

        services.AddSingleton<AgentRouter>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var kernel = provider.GetRequiredService<Kernel>();
            var documentScanner = provider.GetRequiredService<IDocumentScanner>();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var userService = provider.GetRequiredService<IUserService>();
            return new AgentRouter(kernel, configuration, documentScanner, loggerFactory, userService);
        });

        return services;
    }
}

