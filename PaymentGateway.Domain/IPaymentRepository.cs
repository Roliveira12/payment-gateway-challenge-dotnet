namespace PaymentGateway.Infra.Repositories;

public interface IPaymentRepository
{
    public List<PostPaymentResponse> Payments { get; set; }

    public void Add(PostPaymentResponse payment);

    public PostPaymentResponse Get(Guid id);
}