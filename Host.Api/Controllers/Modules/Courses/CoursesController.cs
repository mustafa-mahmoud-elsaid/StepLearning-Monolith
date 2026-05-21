using Courses.Application.DTO;
using Courses.Application.Features.Commands.Courses.DeleteCourse;
using Courses.Application.Features.Commands.Courses.PublishCourse;
using Courses.Application.Features.Commands.Courses.UpdateCourse;
using Courses.Application.Features.Create.Courses;
using Courses.Application.Features.Query.Courses.GetCourseDetails;
using Courses.Application.Features.Query.Courses.GetCoursePreview;
using Courses.Application.Features.Query.Courses.GetInstructorCourses;
using Courses.Application.Features.Query.Courses.SearchCourses;
using Courses.Application.Features.Query.Courses.GetCourseCards;
using Courses.Application.Features.Query.Courses.GetStudentCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Courses;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── Queries ──────────────────────────────────────────────────

    [HttpGet("{id:guid}/preview")]
    public async Task<IActionResult> GetPreview(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCoursePreviewQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("{id:guid}/details")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        if (!User.TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await _mediator.Send(new GetCourseDetailsQuery(id, studentId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("cards")]
    public async Task<IActionResult> GetCards([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetCourseCardsQuery(pageNumber, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SearchCoursesQuery(title, minPrice, maxPrice, pageNumber, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("instructor")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> GetInstructorCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        if (!User.TryGetInstructorId(out var instructorId))
            return Unauthorized("Instructor ID not found in token.");

        var result = await _mediator.Send(new GetInstructorCoursesQuery(instructorId, pageNumber, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetStudentCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        if (!User.TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await _mediator.Send(new GetStudentCoursesQuery(studentId, pageNumber, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // ── Commands ─────────────────────────────────────────────────

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequest request, CancellationToken ct)
    {
        if (!User.TryGetInstructorId(out var instructorId))
            return Unauthorized("Instructor ID not found in token.");

        var dto = new CourseCreateDto(instructorId, request.Title, request.Description);
        var result = await _mediator.Send(new CreateCourseCommand(dto), ct);
        return result.IsSuccess ? CreatedAtAction(nameof(GetPreview), new { id = result.Value }, result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CourseUpdateDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateCourseCommand(id, dto), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        if (!User.TryGetInstructorId(out var instructorId))
            return Unauthorized("Instructor ID not found in token.");

        var result = await _mediator.Send(new PublishCourseCommand(id, instructorId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

public record CourseCreateRequest(string Title, string? Description);
