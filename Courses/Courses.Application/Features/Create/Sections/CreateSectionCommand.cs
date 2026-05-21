using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public record CreateSectionCommand(SectionCreateDto Details) : IRequest<Result<Guid>>;
