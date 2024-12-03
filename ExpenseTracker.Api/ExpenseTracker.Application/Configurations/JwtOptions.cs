using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.Configurations;

public sealed class JwtOptions
{
    public const string SectionName = nameof(JwtOptions);

    [Required]
    [MinLength(1)]
    public required string Audience { get; init; }

    [Required]
    [MinLength(1)]
    public required string Issuer { get; init; }

    [Required]
    [MinLength(15)]
    public required string SecretKey { get; init; }

    [Range(4, 100)]
    public int ExpiresInHours { get; init; }
}
