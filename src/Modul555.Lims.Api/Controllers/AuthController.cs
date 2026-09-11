using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modul555.Lims.Application.Auth;
using Modul555.Lims.Infrastructure.Services;

namespace Modul555.Lims.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct
    )
    {
        var result = await _auth.LoginViaKeycloakAsync(request, ct);
        return result is null
            ? Unauthorized(new { message = "Неверный логин или пароль." })
            : Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken ct
    )
    {
        var result = await _auth.RefreshAsync(request.RefreshToken, ct);
        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> Me(CancellationToken ct)
    {
        var profile = await _auth.GetOrSyncProfileAsync(ct);
        return profile is null ? Unauthorized() : Ok(profile);
    }
}
