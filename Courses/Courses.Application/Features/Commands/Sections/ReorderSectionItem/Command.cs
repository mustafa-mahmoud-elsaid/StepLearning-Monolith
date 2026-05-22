using Courses.Application.DTO;

namespace Courses.Application.Features.Commands.Sections.ReorderSectionItem;

public record ReorderSectionItemCommand(Guid SectionId, ReorderDto Reorder) : IRequest<Result>;
