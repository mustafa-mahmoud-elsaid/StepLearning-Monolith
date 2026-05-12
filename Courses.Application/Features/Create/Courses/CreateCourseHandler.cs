using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Courses;

public class CreateCourseHandler(ICoursesRepository coursesRepository, IInstructorService instructorService) : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly ICoursesRepository _coursesRepository = coursesRepository;
    private readonly IInstructorService _instructorService = instructorService;

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (await _instructorService.Exists(request.dto.InstructorId, cancellationToken))
            return Result<Guid>.Failure("Can not create the course, no intructor with this id");

        var course = Course.Create(request.dto.Title, request.dto.Description, request.dto.InstructorId);

        var courseId = await _coursesRepository.CreateCourseAsync(course, cancellationToken);

        return Result<Guid>.Success(courseId);
    }
}
