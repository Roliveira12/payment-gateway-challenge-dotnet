namespace PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries
{
    public record AuthorizePaymentUseCaseInput
    {
        public AuthorizePaymentUseCaseInput(string cardNumber, string expiryDate, string currency, int amount, int cvv)
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            Currency = currency;
            Amount = amount;
            Cvv = cvv;
        }

        public string CardNumber { get; private set; }
        public string ExpiryDate { get; private set; }
        public string Currency { get; private set; }
        public int Amount { get; private set; }
        public int Cvv { get; private set; }
    }
}