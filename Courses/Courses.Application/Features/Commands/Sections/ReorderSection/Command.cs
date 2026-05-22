using Courses.Application.DTO;

namespace Courses.Application.Features.Commands.Sections.ReorderSection;

public record ReorderSectionCommand(Guid CourseId, ReorderDto Reorder) : IRequest<Result>;
