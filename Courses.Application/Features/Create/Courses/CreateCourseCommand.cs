using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Courses;

public record CreateCourseCommand(CourseCreateDto Details) : IRequest<Result<Guid>>;

