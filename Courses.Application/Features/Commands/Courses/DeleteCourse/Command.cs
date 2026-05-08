using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.DeleteCourse;

public record DeleteCourseCommand(Guid CourseId) : IRequest<Result>;
