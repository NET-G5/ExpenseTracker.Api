namespace ExpenseTracker.Infrastructure.Sms.Models;

public sealed class SmsMessage
{
    public string ToNumber { get; set; }
    public string Message { get; set; }
    public string? Subject { get; set; }

    public SmsMessage(string toNumber, string message)
        : this(toNumber, message, null)
    {

    }

    public SmsMessage(string toNumber, string message, string? subject)
    {
        ToNumber = toNumber;
        Message = message;
        Subject = subject;
    }
}
