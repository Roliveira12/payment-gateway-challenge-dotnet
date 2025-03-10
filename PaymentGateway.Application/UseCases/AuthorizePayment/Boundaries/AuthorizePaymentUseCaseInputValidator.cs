using System.Globalization;

using FluentValidation;

namespace PaymentGateway.Application.UseCases.AuthorizePayment.Boundaries;

public class AuthorizePaymentUseCaseInputValidator : AbstractValidator<AuthorizePaymentUseCaseInput>
{
    private static readonly IReadOnlyCollection<string> SupportedCurrencies = ["USD", "BRL", "EUR"];

    public AuthorizePaymentUseCaseInputValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Card number is required")
            .Matches("^[0 - 9]+$").WithMessage("Card number must contain only numbers")
            .Length(14, 19).WithMessage("Card number must be between 14 and 19 digits");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("ExpiryDate is required")
            .Matches("^([0-9][0-9])\\/\\d{4}$").WithMessage("ExpiryDate must be on format MM/yyyy")
            .Must(BeAValidDate).WithMessage("Expiry Date must be on future");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be exactly 3 characters (e.g., USD, BRL) in accordance to ISO 4217.")
            .Must(SupportedsCurrency).WithMessage($"Only supported currencies is {string.Join("|", SupportedCurrencies)}");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transaction amount must be greater than zero.");

        RuleFor(x => x.Cvv)
            .InclusiveBetween(100, 999).WithMessage("CVV must be 3 or 4 digits.");
    }

    private bool SupportedsCurrency(string currency)
    {
        return SupportedCurrencies.Contains(currency, StringComparer.InvariantCultureIgnoreCase);
    }

    private bool BeAValidDate(string expiryDate)
    {
        if (!DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
        {
            return false;
        }
        var actualDate = DateTime.UtcNow;

        if (parsedDate.Year < actualDate.Year)
        {
            return false;
        }
        else
        {
            return parsedDate.Month >= actualDate.Month || parsedDate.Year != actualDate.Year;
        }
    }
}