using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.ReorderSection;

public record ReorderSectionCommand(Guid CourseId, ReorderDto Reorder) : IRequest<Result>;
