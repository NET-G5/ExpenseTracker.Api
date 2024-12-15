using ExpenseTracker.Application.Requests.Auth;

namespace ExpenseTracker.Application.Interfaces;

public interface INewUserService
{
    Task HandlePostRegistrationAsync(RegisterRequest request);
}
