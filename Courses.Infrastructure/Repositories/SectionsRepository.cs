using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using Courses.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories;

internal class SectionsRepository(CoursesDbContext dbContext) : ISectionsRepository
{
    public async Task<Guid> CreateSectionAsync(Section section, CancellationToken cancellationToken = default)
    {
        await dbContext.Sections.AddAsync(section, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return section.Id;
    }

    public async Task<int> GetLastDisplayOrderAsync(Guid courseId)
    {
        var maxOrder = await dbContext.Sections.AsNoTracking().Where(s => s.CourseId == courseId).Select(s => (int?)s.DisplayOrder).MaxAsync();

        return maxOrder ?? 0;
    }
}
