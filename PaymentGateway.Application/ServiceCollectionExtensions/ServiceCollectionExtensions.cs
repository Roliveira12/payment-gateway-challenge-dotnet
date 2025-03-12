using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Application.UseCases.AuthorizePayment;
using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

namespace PaymentGateway.Application.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddAuthorizePaymentUseCase();

            return services;
        }

        public static IServiceCollection AddAuthorizePaymentUseCase(this IServiceCollection services)
        {
            return services
                           .AddScoped<IValidator<AuthorizePaymentUseCaseInput>, AuthorizePaymentUseCaseInputValidator>()
                           .AddScoped<IAuthorizePaymentUseCase, AuthorizePaymentUseCase>();
        }
    }
}