using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Infrastructure.Email.Models;
using FluentEmail.Core;

namespace ExpenseTracker.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    private readonly IFluentEmailFactory _emailFactory;

    public EmailService(IFluentEmailFactory emailFactory)
    {
        _emailFactory = emailFactory ?? throw new ArgumentNullException(nameof(emailFactory));
    }

    public void SendEmailConfirmation(EmailMessage message, UserInfo userInfo)
    {
        var emailConfirmation = new EmailConfirmation(
            message.Username,
            message.FallbackUrl!,
            userInfo?.OS,
            userInfo?.Browser,
            null!);

        var templatePath = Path.Combine(AppContext.BaseDirectory, "Email\\Templates", "EmailConfirmation.cshtml");

        _emailFactory
            .Create()
            .To(message.To)
            .Subject(message.Subject)
            .UsingTemplateFromFile(templatePath, emailConfirmation)
            .Send();
    }

    public void SendResetPassword(EmailMessage message, UserInfo userInfo)
    {
        var sendResetPassword = new ResetPassword(
            message.Username,
            message.FallbackUrl!,
            userInfo.OS,
            userInfo.Browser,
            null);

        var templatePath = Path.Combine(AppContext.BaseDirectory, "Email\\Templates", "ResetPassword.cshtml");
        _emailFactory
            .Create()
            .To(message.To)
            .Subject(message.Subject)
            .UsingTemplateFromFile(templatePath, sendResetPassword)
            .Send();
    }

    public void SendWelcome(EmailMessage message)
    {
        var sendResetPassword = new Welcome(
            message.Username,
            message.To,
            message.FallbackUrl!,
            DateTime.Now.ToString("dd MMMM, yyyy"),
            DateTime.Now.AddMonths(1).ToString("dd MMMM, yyyy"),
            "30");

        var templatePath = Path.Combine(AppContext.BaseDirectory, "Email\\Templates", "Welcome.cshtml");
        _emailFactory
            .Create()
            .To(message.To)
            .Subject(message.Subject)
            .UsingTemplateFromFile(templatePath, sendResetPassword)
            .Send();
    }
    public void SendInvitation(EmailMessage message)
    {
        throw new NotImplementedException();
    }

    public void SendWalletInvitation(EmailMessage message, string inviteSenderName)
    {
        throw new NotImplementedException();
    }

}
