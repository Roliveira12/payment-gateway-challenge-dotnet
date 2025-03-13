namespace PaymentGateway.Domain.Enums;

public enum PaymentStatus
{
    Authorized,
    InProcessing,
    Declined,
    Rejected
}