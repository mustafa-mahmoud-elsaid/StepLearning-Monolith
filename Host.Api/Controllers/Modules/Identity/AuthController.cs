using Identity.Application.Domain.DTO;
using Identity.Application.Features.Login;
using Identity.Application.Features.RefreshToken;
using Identity.Application.Features.Register.Instructor;
using Identity.Application.Features.Register.Student;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Identity;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new StudentRegisterCommand(dto), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
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
}

public record RefreshTokenRequest(string RefreshToken);
