using System.Net;

namespace PaymentGateway.Application;

public class UseCaseResult<T>(bool sucess, T? data, string? errorMessage, int statusCode)
{
    public bool Sucess { get; set; } = sucess;
    public T? Data { get; set; } = data;
    public string? ErrorMessage { get; set; } = errorMessage;
    public int StatusCode { get; set; } = statusCode;

    public static UseCaseResult<T> UseCaseSucess(T? data) => new(true, data, null, (int)HttpStatusCode.OK);

    public static UseCaseResult<T> BadRequest(string errorMessage) => new(false, default, errorMessage, (int)HttpStatusCode.BadRequest);

    public static UseCaseResult<T> Error(string errorMessage, int statusCode) => new(false, default, errorMessage, statusCode);
}