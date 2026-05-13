using Enrollment.Application.DTO;
using Enrollment.Application.Features.Commands.Enroll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Enrollments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")] // Assuming only students can enroll
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EnrollStudentRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new EnrollStudentCommand(dto), ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
