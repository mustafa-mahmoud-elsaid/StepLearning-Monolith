using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCoursePreview;

public record GetCoursePreviewQuery(Guid Id) : IRequest<Result<CoursePreviewDto>>;
