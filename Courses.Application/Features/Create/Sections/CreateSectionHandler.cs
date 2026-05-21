using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;
    private readonly ICoursesRepository _coursesRepository;

    public CreateSectionHandler(ISectionsRepository sectionsRepository, ICoursesRepository coursesRepository)
    {
        _sectionsRepository = sectionsRepository;
        _coursesRepository = coursesRepository;
    }
    public async Task<Result<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var course = await _coursesRepository.GetCourseByIdEntityAsync(request.Details.CourseId, cancellationToken);
        if (course is null)
            return Result<Guid>.Failure("Course not found.");


        var lastSectionOrder = await _sectionsRepository.GetLastDisplayOrderAsync<Section>(s => s.CourseId == request.Details.CourseId)!;
        var nextOrder = DisplayOrderCalculator.GetNext(lastSectionOrder);

        var section = Section.Create(request.Details.Title, request.Details.CourseId, nextOrder);

        var sectionId = await _sectionsRepository.CreateSectionAsync(section, cancellationToken);

        return Result<Guid>.Success(sectionId);
    }
}
