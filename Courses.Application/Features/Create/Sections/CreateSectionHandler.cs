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


        var lastSectionOrder = await _sectionsRepository.GetLastDisplayOrderAsync<Section>(s => s.Id == request.Details.CourseId)!;

        var section = new Section
        {
            Id = Guid.NewGuid(),
            Title = request.Details.Title,
            CourseId = request.Details.CourseId
        };

        section.DisplayOrder = DisplayOrderCalculator.GetNext(lastSectionOrder);

        var sectionId = await _sectionsRepository.CreateSectionAsync(section, cancellationToken);

        return Result<Guid>.Success(sectionId);
    }
}
