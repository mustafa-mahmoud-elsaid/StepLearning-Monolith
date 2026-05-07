using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using Courses.Infrastructure.Data;

namespace Courses.Infrastructure.Repositories;

internal sealed class CoursesRepository : ICoursesRepository
{
    private readonly CoursesDbContext _dbContext;

    public CoursesRepository(CoursesDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Guid> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        await _dbContext.Courses.AddAsync(course, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}
