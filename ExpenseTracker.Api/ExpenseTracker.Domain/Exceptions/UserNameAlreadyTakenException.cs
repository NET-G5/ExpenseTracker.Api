namespace ExpenseTracker.Domain.Exceptions;

public class UserNameAlreadyTakenException : ApplicationException
{
    public UserNameAlreadyTakenException(string message) : base(message) { }

}