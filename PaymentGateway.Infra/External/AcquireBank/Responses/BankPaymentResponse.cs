namespace PaymentGateway.Infra.External.AcquireBank.Responses;

public class BankPaymentResponse
{
    public bool Authorized { get; set; }
    public string AuthorizationCode { get; set; }
}