using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

public class AuthorizePaymentUseCaseOutput
{
    public string Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public string ExpiryDate { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}