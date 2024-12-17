using ExpenseTracker.Application.Models;

namespace ExpenseTracker.Application.Interfaces;

public interface IEmailService
{
    void SendWelcome(EmailMessage message);
    void SendInvitation(EmailMessage message);
    void SendWalletInvitation(EmailMessage message, string inviteSenderName);
    void SendEmailConfirmation(EmailMessage message, UserInfo? userInfo);
    void SendResetPassword(EmailMessage message, UserInfo? userInfo);
}

