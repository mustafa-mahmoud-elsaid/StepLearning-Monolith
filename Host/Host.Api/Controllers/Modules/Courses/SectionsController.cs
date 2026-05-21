using Courses.Application.DTO;
using Courses.Application.Features.Commands.Sections.ReorderSection;
using Courses.Application.Features.Commands.Sections.ReorderSectionItem;
using Courses.Application.Features.Commands.Sections.UpdateSection;
using Courses.Application.Features.Commands.Sections.UpdateSectionItem;
using Courses.Application.Features.Create.Sections;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Courses;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Instructor")]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── Create ───────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> CreateSection([FromBody] SectionCreateDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSectionCommand(dto), ct);
        return result.IsSuccess ? Created($"/api/sections/{result.Value}", result.Value) : BadRequest(result.Error);
    }

    [HttpPost("items/video")]
    public async Task<IActionResult> CreateVideoItem([FromBody] VideoCreateDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateVideoItemCommand(dto), ct);
        return result.IsSuccess ? Created($"/api/sections/items/{result.Value}", result.Value) : BadRequest(result.Error);
    }

    [HttpPost("items/pdf")]
    public async Task<IActionResult> CreatePdfItem([FromBody] PdfCreateDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreatePdfItemCommand(dto), ct);
        return result.IsSuccess ? Created($"/api/sections/items/{result.Value}", result.Value) : BadRequest(result.Error);
    }

    // ── Update ───────────────────────────────────────────────────

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateSection(Guid id, [FromBody] UpdateSectionRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSectionCommand(id, request.Title), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("items/{id:guid}")]
    public async Task<IActionResult> UpdateSectionItem(Guid id, [FromBody] UpdateSectionItemRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSectionItemCommand(id, request.Title), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    // ── Reorder ──────────────────────────────────────────────────

    [HttpPost("reorder/{courseId:guid}")]
    public async Task<IActionResult> ReorderSection(Guid courseId, [FromBody] ReorderDto reorder, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReorderSectionCommand(courseId, reorder), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("items/reorder/{sectionId:guid}")]
    public async Task<IActionResult> ReorderSectionItem(Guid sectionId, [FromBody] ReorderDto reorder, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReorderSectionItemCommand(sectionId, reorder), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

// ── Request DTOs for PATCH endpoints ──────────────────────────────
public record UpdateSectionRequest(string? Title);
public record UpdateSectionItemRequest(string? Title);
