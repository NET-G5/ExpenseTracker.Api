using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.Configurations;

public sealed class TokenSettings
{
    public const string SectionName = nameof(TokenSettings);

    [Required]
    [MinLength(1)]
    public required string Audience { get; init; }

    [Required]
    [MinLength(1)]
    public required string Issuer { get; init; }

    [Required]
    [MinLength(15)]
    public required string SecretKey { get; init; }

    [Range(1, 24, ErrorMessage = "Access token expiration must be between 1 hour and 24 hours")]
    public int JwtExpiresInHours { get; init; }

    [Range(1, 30, ErrorMessage = "Refresh token expiration must be between 1 day and 30 days")]
    public int RefreshExpiresInDays { get; init; }
}
