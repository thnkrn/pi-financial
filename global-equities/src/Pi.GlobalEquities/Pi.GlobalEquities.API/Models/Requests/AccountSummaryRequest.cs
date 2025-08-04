using System.ComponentModel.DataAnnotations;
using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.API.Models.Requests;

public class AccountSummaryRequest : IValidatableObject
{
    [Required]
    public string AccountId { get; init; }
    [Required]
    public Currency[] Currencies { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Currencies.Length == 0)
            yield return new ValidationResult("At least one currency is required.");
        if (Currencies.Length > 2)
            yield return new ValidationResult("The maximum number of currencies that can be requested is 2.");
        if (Currencies.Any(x => x != Currency.USD && x != Currency.HKD))
            yield return new ValidationResult("Unsupported Currency.");
    }
}
