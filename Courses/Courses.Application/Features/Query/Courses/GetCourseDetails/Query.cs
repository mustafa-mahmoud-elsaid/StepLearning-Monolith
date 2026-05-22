using Courses.Application.DTO;

namespace Courses.Application.Features.Query.Courses.GetCourseDetails;

public record GetCourseDetailsQuery(Guid CourseId, Guid StudentId) : IRequest<Result<CourseDetailsDto>>;
