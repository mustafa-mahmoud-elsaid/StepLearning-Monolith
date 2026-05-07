using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Courses;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly ICoursesRepository _coursesRepository;

    public CreateCourseHandler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }
    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        // TODO: check instructor id

        var course = Course.Create(request.dto.Title, request.dto.Description, request.dto.InstructorId);

        var courseId = await _coursesRepository.CreateCourseAsync(course, cancellationToken);

        return Result<Guid>.Success(courseId);
    }
}
