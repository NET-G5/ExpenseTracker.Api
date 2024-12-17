using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Application.Configurations;

public class EmailOptions
{
    public const string SectionName = nameof(EmailOptions);

    [Required(ErrorMessage = "From Email is required")]
    public required string FromEmail { get; init; }

    [Required(ErrorMessage = "From Name is required")]
    public required string FromName { get; init; }

    [Required(ErrorMessage = "Host is required")]
    public required string Server { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Port is required")]
    public int Port { get; set; }
}
