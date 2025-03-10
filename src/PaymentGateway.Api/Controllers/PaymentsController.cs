using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Application.UseCases.AuthorizePayment;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentsController(PaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        return new OkObjectResult(payment);
    }

    [HttpPost("submit-payment")]
    public async Task<IActionResult> SubmitPaymentAsync(
        [FromServices] IAuthorizePaymentUseCase submitPaymentUseCase,
        [FromBody] PostPaymentRequest postPaymentRequest)
    {
        var input = postPaymentRequest.ToInput();
        var useCaseResult = await submitPaymentUseCase.ExecuteAsync(input);

        return Ok(useCaseResult.Data);
    }
}