using PaymentGateway.Infra.External.AcquireBank.Requests;
using PaymentGateway.Infra.External.AcquireBank.Responses;
using Refit;

namespace PaymentGateway.Infra.External.AcquireBank;

public interface IAcquireBankApi
{
    [Post("payments")]
    Task<ApiResponse<BankPaymentResponse>> AuthorizePaymentAsync([Body] BankPaymentRequest bankPaymentRequest);
}