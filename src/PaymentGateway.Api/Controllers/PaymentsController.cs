using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Application.UseCases.AuthorizePayment;
using PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

using Swashbuckle.AspNetCore.Annotations;

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


    [HttpPost("authorize")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, "Payment Authorization", typeof(AuthorizePaymentUseCaseOutput))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Input with error", typeof(ProblemDetails))]       
    public async Task<IActionResult> SubmitPaymentAsync(
        [FromServices] IAuthorizePaymentUseCase submitPaymentUseCase,
        [FromBody] PostPaymentRequest postPaymentRequest)
    {
        var input = postPaymentRequest.ToInput();
        var useCaseResult = await submitPaymentUseCase.ExecuteAsync(input);

        if (!useCaseResult.Sucess)
        {
            return Problem(detail: useCaseResult.ErrorMessage, statusCode: useCaseResult.StatusCode);

        }
        return Ok(useCaseResult.Data);
    }
}