using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;

public record UpdateCourseCommand(Guid CourseId, Guid InstructorId, CourseUpdateDto Details) : IRequest<Result>;
