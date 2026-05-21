using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Courses;

internal sealed class CreateCourseHandler(ICoursesRepository coursesRepository, IInstructorService instructorService) : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly ICoursesRepository _coursesRepository = coursesRepository;
    private readonly IInstructorService _instructorService = instructorService;

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (await _instructorService.Exists(request.Details.InstructorId, cancellationToken))
            return Result<Guid>.Failure("Cannot create the course. No instructor found with this ID.");

        var course = Course.Create(request.Details.Title, request.Details.Description, request.Details.InstructorId);

        var courseId = await _coursesRepository.CreateCourseAsync(course, cancellationToken);

        return Result<Guid>.Success(courseId);
    }
}
