using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.Application.Extensions;
public static class ValidationProblemDetailsExtensions
{
    public static void AddValidationErrors(this ValidationProblemDetails? problemDetails, IEnumerable<ValidationFailure> errors)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        ArgumentNullException.ThrowIfNull(errors);

        var groupedErrors = errors
            .GroupBy(x => x.PropertyName)
            .Select(x => x)
            .ToList();

        foreach (var groupedError in groupedErrors)
        {
            if (string.IsNullOrWhiteSpace(groupedError.Key))
            {
                problemDetails.Errors.Add("General", [.. groupedError.Select(x => x.ErrorMessage)]);
                continue;
            }

            problemDetails.Errors.Add(groupedError.Key, [.. groupedError.Select(x => x.ErrorMessage)]);
        }
    }
}
