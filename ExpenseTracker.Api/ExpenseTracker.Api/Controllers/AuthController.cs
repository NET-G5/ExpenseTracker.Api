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
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Login to get JWT token.
    /// </summary>
    /// <param name="request">Username and password to login</param>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);

        return Ok(token);
    }


    /// <summary>
    /// Register to create a new user.
    /// </summary>
    /// <param name="request">Register to create a new user.</param>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RegisterAsync(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);

        return NoContent();
    }

    /// <summary>
    /// Confirm email
    /// </summary>
    /// <param name="request">Email and token to confirm.</param>
    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ConfirmEmailAsync(EmailConfirmationRequest request)
    {
        await _authService.ConfirmEmailAsync(request);

        return NoContent();
    }

    /// <summary>
    /// Reset Password
    /// </summary>
    /// <param name="request">Parameters to reset password</param>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request);

        return NoContent();
    }

    /// <summary>
    /// Confirm reset password
    /// </summary>
    /// <param name="request">Parameters to confirm a new password</param>
    [HttpPost("reset-password-confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request)
    {
        await _authService.ConfirmResetPasswordAsync(request);

        return NoContent();
    }
}
