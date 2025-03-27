using Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPaymentService>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var httpClient = new HttpClient();
            var transactionsApiUrl = configuration["BackendAPIs:TransactionsApiUrl"];
            return new PaymentService(loggerFactory.CreateLogger<PaymentService>(), httpClient, transactionsApiUrl);
        });
        return services;
    }
}
