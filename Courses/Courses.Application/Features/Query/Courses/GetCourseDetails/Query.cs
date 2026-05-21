using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCourseDetails;

public record GetCourseDetailsQuery(Guid CourseId, Guid StudentId) : IRequest<Result<CourseDetailsDto>>;
