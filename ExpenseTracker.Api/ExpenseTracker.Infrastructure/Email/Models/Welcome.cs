namespace ExpenseTracker.Infrastructure.Email.Models;

public class Welcome
{
    public string UserName { get; }
    public string Email { get; }
    public string ActionUrl { get; }
    public string TrialStartDate { get; }
    public string TrialEndDate { get; }
    public string TrialLength { get; }
    public string? SupportEmail { get; }
    public string? HelpUrl { get; set; }
    public string? LiveChatUrl { get; }

    public Welcome(string userName, string email, string actionUrl, string trialStartDate, string trialEndDate, string trialLength)
    {
        UserName = userName;
        Email = email;
        ActionUrl = actionUrl;
        TrialStartDate = trialStartDate;
        TrialEndDate = trialEndDate;
        TrialLength = trialLength;

    }
}

