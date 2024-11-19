namespace ExpenseTracker.Domain.Exceptions;

public sealed class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
}
