using Enrollment.Application.Domain.Enums;
using Enrollment.Application.DTO;
using Enrollment.Application.Features.Commands.Enroll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Enrollments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")] // Only students can enroll
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirst("studentId")?.Value, out var studentId))
            return Unauthorized("Student ID not found in token.");

        var dto = new EnrollStudentRequestDto(studentId, request.CourseId, request.PaymentId, request.Status);
        var result = await _mediator.Send(new EnrollStudentCommand(dto), ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public record EnrollRequest(Guid CourseId, Guid PaymentId, EnrollmentStatus Status = EnrollmentStatus.Active);
