using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Entities
{
    public class PostPaymentDto
    {
        public string Id { get; set; }
        public PaymentStatus Status { get; set; }
        public string CardNumberLastFour { get; set; }
        public string ExpireDate { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
    }
}