using Courses.Application.RepositoriesContracts;
using StepLearning.Shared.Abstraction;

namespace Courses.Infrastructure.Services;

internal class CourseService(ICoursesRepository coursesRepository) : ICourseService
{
    public async Task<bool> Exists(Guid courseId, CancellationToken ct = default)
    {
        if(courseId == Guid.Empty) 
            return false;

        return await coursesRepository.Exists(courseId, ct);
    }
}
