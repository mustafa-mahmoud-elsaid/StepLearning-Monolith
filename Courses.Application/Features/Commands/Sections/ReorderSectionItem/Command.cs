using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.ReorderSectionItem;

public record ReorderSectionItemCommand(Guid SectionId, ReorderDto Reorder) : IRequest<Result>;
