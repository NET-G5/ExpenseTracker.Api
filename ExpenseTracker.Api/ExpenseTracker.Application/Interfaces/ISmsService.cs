using ExpenseTracker.Infrastructure.Sms.Models;

namespace ExpenseTracker.Application.Interfaces;

public interface ISmsService
{
    Task SendMessage(SmsMessage message);
}
