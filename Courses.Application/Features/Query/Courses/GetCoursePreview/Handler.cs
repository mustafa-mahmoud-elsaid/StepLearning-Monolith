using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCoursePreview;

public class Handler : IRequestHandler<GetCoursePreviewQuery, Result<CoursePreviewDto>>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result<CoursePreviewDto>> Handle(GetCoursePreviewQuery request, CancellationToken cancellationToken)
    {
        var coursePreview = await _coursesRepository.GetCoursePreviewAsync(request.Id, cancellationToken);

        return coursePreview is null
            ? Result<CoursePreviewDto>.Failure("Course not found")
            : Result<CoursePreviewDto>.Success(coursePreview);
    }
}
