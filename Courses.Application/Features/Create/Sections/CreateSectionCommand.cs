using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public record CreateSectionCommand(SectionCreateDto dto) : IRequest<Result<Guid>>;
