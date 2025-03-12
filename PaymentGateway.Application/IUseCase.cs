namespace PaymentGateway.Application
{
    public interface IUseCase<TInput, TOutput> 
        where TInput : class
        where TOutput : class
    {
        Task<UseCaseResult<TOutput>> ExecuteAsync(TInput input);
    }
}