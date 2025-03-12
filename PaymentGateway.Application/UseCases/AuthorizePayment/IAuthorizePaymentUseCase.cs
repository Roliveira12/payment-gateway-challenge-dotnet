using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

namespace PaymentGateway.Application.UseCases.AuthorizePayment;

public interface IAuthorizePaymentUseCase : IUseCase<AuthorizePaymentUseCaseInput, AuthorizePaymentUseCaseOutput>
{
}