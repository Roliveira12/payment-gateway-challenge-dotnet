namespace PaymentGateway.Application.UseCases.AuthorizePayment
{
    public static class AuthorizePaymentErrorConstants
    {
        public static string INTERNAL_ERROR_TO_AUTHORIZE = "Internal error to authorize payment, please try again.";
        public static string EXTERNAL_ERROR_TO_AUTHORIZE = "The request could not be processed due to a temporary issue. Please try again.";

    }
}