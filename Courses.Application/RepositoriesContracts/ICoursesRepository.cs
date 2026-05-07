using Courses.Domain.Entities;

namespace Courses.Application.RepositoriesContracts;

public interface ICoursesRepository
{
    Task<Guid> CreateCourseAsync(Course course, CancellationToken cancellationToken = default);
}
