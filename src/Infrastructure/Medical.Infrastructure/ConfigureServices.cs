using Medical.Application.Contracts.Payment;
using Medical.Application.Model;
using Medical.Infrastructure.Services.PaymentService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Medical.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeConfig>(configuration.GetSection("StripeSettings"));

        services.Configure<AppConfig>(configuration.GetSection("AppConfig"));

        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
