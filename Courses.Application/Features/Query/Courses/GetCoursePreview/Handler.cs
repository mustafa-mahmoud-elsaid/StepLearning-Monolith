using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCoursePreview;
internal sealed class Handler(ICoursesRepository coursesRepository) : IRequestHandler<GetCoursePreviewQuery, Result<CoursePreviewDto>>
{

    public async Task<Result<CoursePreviewDto>> Handle(GetCoursePreviewQuery request, CancellationToken cancellationToken)
    {
        var coursePreview = await coursesRepository.GetCoursePreviewAsync(request.Id, cancellationToken);

        return coursePreview is null
            ? Result<CoursePreviewDto>.Failure("Course not found")
            : Result<CoursePreviewDto>.Success(coursePreview);
    }
}


