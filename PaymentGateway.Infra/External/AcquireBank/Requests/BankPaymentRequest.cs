namespace PaymentGateway.Infra.External.AcquireBank.Requests;

public class BankPaymentRequest
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public int Cvv { get; set; }
}