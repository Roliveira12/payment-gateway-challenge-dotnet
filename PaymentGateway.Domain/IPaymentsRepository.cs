using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infra.Repositories;

public interface IPaymentsRepository
{
    public void Add(PostPaymentDto payment);

    public Task<PostPaymentDto?> GetAsync(Guid id);
}