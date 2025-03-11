using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Infra.External.AcquireBank;

using Refit;

namespace PaymentGateway.Infra.External
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddGatewayApis(this IServiceCollection services) {

            
            services.AddRefitClient<IAcquireBankApi>(provider =>
            {
                provider.GetRequiredService<IAcquireBankApi>()
            })
                    .ConfigureHttpClient(cfg =>
                    {
                        cfg.Se
                        cfg.BaseAddress
                    })

        }
    }
}
