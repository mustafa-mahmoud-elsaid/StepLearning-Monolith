using Identity.Application.DTO;
using Identity.Application.Features.Login;
using Identity.Application.Features.Logout;
using Identity.Application.Features.RefreshToken;
using Identity.Application.Features.Register.Instructor;
using Identity.Application.Features.Register.Student;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StepLearning.Shared.Abstraction;

namespace Host.Api.Controllers.Modules.Identity;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICartMigrationService _cartMigrationService;

    public AuthController(IMediator mediator, ICartMigrationService cartMigrationService)
    {
        _mediator = mediator;
        _cartMigrationService = cartMigrationService;
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new StudentRegisterCommand(dto), ct);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        // Migrate guest cart if the cookie exists
        const string cookieName = "guest-cart-id";
        if (Request.Cookies.TryGetValue(cookieName, out var guestId) && !string.IsNullOrWhiteSpace(guestId))
        {
            var guestCartKey = $"cart:guest:${guestId}";
            var userId = result.Value!.UserId;

            await _cartMigrationService.MigrateGuestCartAsync(guestCartKey, userId, ct);

            Response.Cookies.Delete(cookieName);
        }

        return Ok(result.Value);
    }

    [HttpPost("register/instructor")]
    public async Task<IActionResult> RegisterInstructor([FromBody] InstructorRegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new InstructorRegisterCommand(dto), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(dto), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LogoutCommand(request.RefreshToken), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

public record RefreshTokenRequest(string RefreshToken);
