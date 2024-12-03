using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Requests.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);

        return Ok(token);
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);

        return NoContent();
    }
}
