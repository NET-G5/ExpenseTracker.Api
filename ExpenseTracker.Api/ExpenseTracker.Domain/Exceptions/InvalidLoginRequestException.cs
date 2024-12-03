namespace ExpenseTracker.Domain.Exceptions;

public sealed class InvalidLoginRequestException : ApplicationException
{
    public InvalidLoginRequestException(string message) : base(message) { }
}