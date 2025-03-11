namespace PaymentGateway.Application.UseCases.AuthorizePayment;

using FluentValidation;

using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;
using PaymentGateway.Infra.External.AcquireBank;
using PaymentGateway.Infra.External.AcquireBank.Requests;

public sealed class AuthorizePaymentUseCase : IAuthorizePaymentUseCase
{
    private readonly IValidator<AuthorizePaymentUseCaseInput> validator;
    private readonly IAcquireBankApi acquireBankApi;

    public AuthorizePaymentUseCase(IValidator<AuthorizePaymentUseCaseInput> validator, IAcquireBankApi acquireBankApi)
    {
        this.validator = validator;
        this.acquireBankApi = acquireBankApi;
    }

    public async Task<UseCaseResult<AuthorizePaymentUseCaseOutput>> ExecuteAsync(AuthorizePaymentUseCaseInput input)
    {
        //validate

        var validationResult = await validator.ValidateAsync(input);

        if (!validationResult.IsValid)
        {
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.BadRequest(string.Join("|", validationResult.Errors));
        }

        //submit to acquireBank

        var bankRequest = new BankPaymentRequest()
        {
            Amount = input.Amount,
            CardNumber = input.CardNumber,
            Currency = input.Currency,
            Cvv = input.Cvv,
            ExpiryDate = input.ExpiryDate,
        };

        var response = await acquireBankApi.AuthorizePaymentAsync(bankRequest);

        if (response.IsSuccessStatusCode)
        {
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.UseCaseSucess(new AuthorizePaymentUseCaseOutput()
            {
                Amount = input.Amount,
                CardNumberLastFour = "xxxx",
                Currency = input.Currency,
                ExpiryDate = input.ExpiryDate,
                Id = response.Content!.AuthorizationCode,
                Status = response.Content.Authorized ? Domain.Enums.PaymentStatus.Authorized : Domain.Enums.PaymentStatus.Declined,

            });
        }

        return UseCaseResult<AuthorizePaymentUseCaseOutput>.InternalServerError("Error", 500);
    }
}