namespace ExpenseTracker.Infrastructure.Email.Models;

public class ResetPassword
{
    public string UserName { get; }
    public string ActionUrl { get; }
    public string? OS { get; }
    public string? Browser { get; }
    public string? SupportUrl { get; }

    public ResetPassword(string userName, string actionUrl)
    {
        UserName = userName;
        ActionUrl = actionUrl;
    }

    public ResetPassword(string userName, string actionUrl, string os, string browser, string supportUrl)
    {
        UserName = userName;
        ActionUrl = actionUrl;
        OS = os;
        Browser = browser;
        SupportUrl = supportUrl;

    }
}
