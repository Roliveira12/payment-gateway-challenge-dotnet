using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using PaymentGateway.Infra.External.AcquireBank;
using PaymentGateway.Infra.External.AcquireBank.Configurations;
using PaymentGateway.Infra.Repositories;

using Refit;

namespace PaymentGateway.Infra.External
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGatewayConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AcquireBankApiConfiguration>(configuration.GetSection("Gateways:AcquireBankApiConfiguration"));

            return services;
        }

        public static IServiceCollection AddGatewayApis(this IServiceCollection services)
        {
            services.AddRefitClient<IAcquireBankApi>()
                    .ConfigureHttpClient((sp, client) =>
                    {
                        var configuration = sp.GetRequiredService<IOptions<AcquireBankApiConfiguration>>().Value;
                        client.BaseAddress = new Uri(configuration.BaseUrl);
                        client.Timeout = TimeSpan.FromMilliseconds(configuration.TimeoutMs);
                    });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

            return services;
        }
    }
}