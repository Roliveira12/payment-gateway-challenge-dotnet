using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infra.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private static List<PostPaymentDto> Payments = new();

    public void Add(PostPaymentDto payment)
    {
        Payments.Add(payment);
    }

    public async Task<PostPaymentDto?> GetAsync(Guid id)
    {
        return Payments.FirstOrDefault(x => x.Id == id.ToString());
    }
}