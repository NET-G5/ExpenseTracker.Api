using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.Configurations;

public sealed class SmsOptions
{
    public const string SectionName = nameof(SmsOptions);

    [Required(ErrorMessage = "SMS Token is required.")]
    public required string Token { get; init; }

    [Required(ErrorMessage = "Url is required.")]
    public required string Url { get; set; }

    [Required(ErrorMessage = "From Number is required.")]
    public int FromNumber { get; init; }
}
