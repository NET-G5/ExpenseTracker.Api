using AutoFixture;

namespace ExpenseTracker.Tests.Unit.Controllers;

public abstract class ControllerTestsBase
{
    protected readonly Fixture _fixture;

    public ControllerTestsBase()
    {
        _fixture = new Fixture();
    }
}
