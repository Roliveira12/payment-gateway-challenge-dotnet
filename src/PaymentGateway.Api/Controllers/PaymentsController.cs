using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Application.UseCases.AuthorizePayment;
using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;
using PaymentGateway.Infra.Repositories;

using Swashbuckle.AspNetCore.Annotations;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, "PaymentDetails", typeof(PostPaymentResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "NotFound")]
    public async Task<IActionResult> GetPaymentAsync([FromServices] IPaymentsRepository _paymentsRepository, Guid id)
    {
        var payment = await _paymentsRepository.GetAsync(id);

        return payment is not null ? new OkObjectResult(payment) : NotFound();
    }

    [HttpPost("authorize")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, "Payment Authorization", typeof(AuthorizePaymentUseCaseOutput))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Input with error", typeof(ProblemDetails))]
    public async Task<IActionResult> SubmitPaymentAsync(
        [FromServices] IAuthorizePaymentUseCase _submitPaymentUseCase,
        [FromBody] PostPaymentRequest postPaymentRequest)
    {
        var input = postPaymentRequest.ToInput();
        var useCaseResult = await _submitPaymentUseCase.ExecuteAsync(input);

        if (!useCaseResult.Sucess)
        {
            return Problem(detail: useCaseResult.ErrorMessage, statusCode: useCaseResult.StatusCode);
        }
        return Ok(useCaseResult.Data);
    }
}