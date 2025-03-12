using System;
using System.Net;

using FluentValidation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using PaymentGateway.Application.UseCases.AuthorizePayment;
using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Infra.External.AcquireBank;
using PaymentGateway.Infra.External.AcquireBank.Requests;
using PaymentGateway.Infra.External.AcquireBank.Responses;
using PaymentGateway.Infra.Repositories;

using Refit;

namespace PaymentGateway.Api.Tests
{
    public class AuthorizePaymentUseCaseTests
    {
        private readonly IAuthorizePaymentUseCase _authorizePaymentUseCase;
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly ILogger<AuthorizePaymentUseCase> _logger;
        private readonly IValidator<AuthorizePaymentUseCaseInput> _validator;
        private readonly IAcquireBankApi _acquireBankApi;

        public AuthorizePaymentUseCaseTests()
        {
            _validator = new AuthorizePaymentUseCaseInputValidator();
            _acquireBankApi = Substitute.For<IAcquireBankApi>();
            _logger = new NullLoggerFactory().CreateLogger<AuthorizePaymentUseCase>();
            _paymentsRepository = Substitute.For<IPaymentsRepository>();

            _authorizePaymentUseCase = new AuthorizePaymentUseCase(_validator, _acquireBankApi, _logger, _paymentsRepository);
        }

        [Theory]
        [InlineData("", "12/2026", "USD", 100, 123, "Card number is required")]
        [InlineData("123abc", "12/2026", "USD", 100, 123, "Card number must contain only numbers")]
        [InlineData("1234567", "12/2026", "USD", 100, 123, "Card number must be between 14 and 19 digits")]
        [InlineData("12345678901234567890", "12/2026", "USD", 100, 123, "Card number must be between 14 and 19 digits")]
        [InlineData("1234567890123456", "", "USD", 100, 123, "ExpiryDate is required")]
        [InlineData("1234567890123456", "13-2026", "USD", 100, 123, "ExpiryDate must be on format MM/yyyy")]
        [InlineData("1234567890123456", "12/2020", "USD", 100, 123, "ExpiryDate must be on future")]
        [InlineData("1234567890123456", "12/2026", "", 100, 123, "Currency is required.")]
        [InlineData("1234567890123456", "12/2026", "ABC", 100, 123, "Only supported currencies is USD|BRL|EUR|GPB")]
        [InlineData("1234567890123456", "12/2026", "USD", -1, 123, "Transaction amount must be greater than zero.")]
        [InlineData("1234567890123456", "12/2026", "USD", 100, 12, "CVV must be 3 or 4 digits.")]
        public async Task Should_Return_BadRequest_When_Validation_Fails(string cardNumber, string expiryDate, string currency, int amount, int cvv, string expectedMessage)
        {
            // Arrange
            var input = new AuthorizePaymentUseCaseInput(cardNumber, expiryDate, currency, amount, cvv);

            // Act
            var result = await _authorizePaymentUseCase.ExecuteAsync(input);

            // Assert
            Assert.False(result.Sucess);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(expectedMessage, result.ErrorMessage);
        }

        [Fact]
        public async Task Should_Return_Success_When_Authorization_Is_Successful()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var input = new AuthorizePaymentUseCaseInput("1234567890123458", "12/2026", "USD", 100, 123);
            var bankResponse = new ApiResponse<BankPaymentResponse>(new HttpResponseMessage(), new BankPaymentResponse()
            {
                AuthorizationCode = guid,
                Authorized = true,
            }, new RefitSettings());

            _acquireBankApi.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>()).Returns(bankResponse);

            // Act
            var result = await _authorizePaymentUseCase.ExecuteAsync(input);

            // Assert
            Assert.True(result.Sucess);
            Assert.Equal(guid, result.Data.Id);
            Assert.Equal(Domain.Enums.PaymentStatus.Authorized, result.Data.Status);
            _paymentsRepository.Received(1).Add(Arg.Any<PostPaymentDto>());

        }

        [Fact]
        public async Task Should_Return_Success_When_Authorization_Is_Declined()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var input = new AuthorizePaymentUseCaseInput("1234567890123458", "12/2026", "USD", 100, 123);
            var bankResponse = new ApiResponse<BankPaymentResponse>(new HttpResponseMessage(), new BankPaymentResponse()
            {
                AuthorizationCode = string.Empty,
                Authorized = false,
            }, new RefitSettings());

            _acquireBankApi.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>()).Returns(bankResponse);

            // Act
            var result = await _authorizePaymentUseCase.ExecuteAsync(input);

            // Assert
            Assert.True(result.Sucess);
            Assert.Empty(result.Data.Id);
            Assert.Equal(Domain.Enums.PaymentStatus.Declined, result.Data.Status);
            _paymentsRepository.Received(0).Add(Arg.Any<PostPaymentDto>());
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable, HttpStatusCode.BadGateway)]

        public async Task Should_Handle_Error_When_BankApiReturnsError(HttpStatusCode statusCodeToReturn, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var input = new AuthorizePaymentUseCaseInput("1234567890123455", "12/2026", "USD", 100, 123);
            var bankResponse = new ApiResponse<BankPaymentResponse>(new HttpResponseMessage()
            {
                StatusCode = statusCodeToReturn
            }, new BankPaymentResponse()
            {
                AuthorizationCode = guid,
                Authorized = false,
            }, new RefitSettings(), await ApiException.Create(new HttpRequestMessage(), HttpMethod.Post, new HttpResponseMessage()
            {
                StatusCode = statusCodeToReturn
            }, new RefitSettings()));

            _acquireBankApi.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>()).Returns(bankResponse);


            // Act
            var result = await _authorizePaymentUseCase.ExecuteAsync(input);

            // Assert
            Assert.False(result.Sucess);
            Assert.Equal((int)expectedStatusCode, result.StatusCode);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}