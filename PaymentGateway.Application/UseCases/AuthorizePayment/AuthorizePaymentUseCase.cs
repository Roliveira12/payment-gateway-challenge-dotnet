namespace PaymentGateway.Application.UseCases.AuthorizePayment;

using System.Net;

using FluentValidation;

using Microsoft.Extensions.Logging;

using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Infra.External.AcquireBank;
using PaymentGateway.Infra.External.AcquireBank.Requests;
using PaymentGateway.Infra.External.AcquireBank.Responses;
using PaymentGateway.Infra.Repositories;

using Refit;

public sealed class AuthorizePaymentUseCase : IAuthorizePaymentUseCase
{
    private readonly IValidator<AuthorizePaymentUseCaseInput> _validator;
    private readonly ILogger<AuthorizePaymentUseCase> _logger;
    private readonly IAcquireBankApi _acquireBankApi;
    private readonly IPaymentsRepository _paymentsRepository;

    public AuthorizePaymentUseCase(IValidator<AuthorizePaymentUseCaseInput> validator, IAcquireBankApi acquireBankApi, ILogger<AuthorizePaymentUseCase> logger, IPaymentsRepository paymentsRepository)
    {
        _validator = validator;
        _acquireBankApi = acquireBankApi;
        _logger = logger;
        _paymentsRepository = paymentsRepository;
    }

    public async Task<UseCaseResult<AuthorizePaymentUseCaseOutput>> ExecuteAsync(AuthorizePaymentUseCaseInput input)
    {
        //validate

        var validationResult = await _validator.ValidateAsync(input);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Input provided is not valid input = {@input}, Error = {@errors}", input, string.Join(" | ", validationResult.Errors));
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.BadRequest(string.Join(" | ", validationResult.Errors));
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

        var response = await _acquireBankApi.AuthorizePaymentAsync(bankRequest);

        if (response.IsSuccessStatusCode)
        {
            if (response.Content!.Authorized)
            {
                var paymentBaseDto = new PostPaymentDto()
                {
                    Amount = input.Amount,
                    CardNumberLastFour = input.CardNumber[^4..],
                    Currency = input.Currency,
                    ExpireDate = input.ExpiryDate,
                    Id = response.Content.AuthorizationCode,
                    Status = response.Content.Authorized ? Domain.Enums.PaymentStatus.Authorized : Domain.Enums.PaymentStatus.Declined,
                };

                _paymentsRepository.Add(paymentBaseDto);
            }
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.UseCaseSucess(new AuthorizePaymentUseCaseOutput()
            {
                Amount = input.Amount,
                CardNumberLastFour = input.CardNumber[^4..],
                Currency = input.Currency,
                ExpiryDate = input.ExpiryDate,
                Id = response.Content!.AuthorizationCode,
                Status = response.Content.Authorized ? Domain.Enums.PaymentStatus.Authorized : Domain.Enums.PaymentStatus.Declined,
            });
        }

        return HandleAuthorizationError(response);
    }

    private UseCaseResult<AuthorizePaymentUseCaseOutput> HandleAuthorizationError(ApiResponse<BankPaymentResponse> response)
    {
        _logger.LogError(response.Error, "Unable to process the payment authorization");

        //If we receive a BadRequest error, is correct to say we send incorrect information to the bank, so we just return a 500 generic error to the client with internal server error, and log it to see whats happening

        if (response.StatusCode is HttpStatusCode.BadRequest)
        {
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.Error(AuthorizePaymentErrorConstants.INTERNAL_ERROR_TO_AUTHORIZE, (int)HttpStatusCode.InternalServerError);
        }

        //If Bank is unaviable we return Bad Gateway, since the error is not in the PaymentGateway, but is in the BankApi
        if (response.StatusCode is HttpStatusCode.ServiceUnavailable)
        {
            return UseCaseResult<AuthorizePaymentUseCaseOutput>.Error(AuthorizePaymentErrorConstants.INTERNAL_ERROR_TO_AUTHORIZE, (int)HttpStatusCode.BadGateway);
        }

        return UseCaseResult<AuthorizePaymentUseCaseOutput>.Error(AuthorizePaymentErrorConstants.INTERNAL_ERROR_TO_AUTHORIZE, (int)response.StatusCode);
    }
}