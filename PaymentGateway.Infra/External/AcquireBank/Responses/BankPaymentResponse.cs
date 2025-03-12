using System.Text.Json.Serialization;

namespace PaymentGateway.Infra.External.AcquireBank.Responses;

public class BankPaymentResponse
{
    public bool Authorized { get; set; }
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}