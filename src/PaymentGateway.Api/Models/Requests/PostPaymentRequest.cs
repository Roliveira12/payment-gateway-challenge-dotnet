using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; } //Must be on format MM/yyyy
    public string Currency { get; set; }
    public int Amount { get; set; }
    public int Cvv { get; set; }

    public AuthorizePaymentUseCaseInput ToInput()
    {
        return new AuthorizePaymentUseCaseInput(CardNumber, ExpiryDate, Currency, Amount, Cvv);
    }
}