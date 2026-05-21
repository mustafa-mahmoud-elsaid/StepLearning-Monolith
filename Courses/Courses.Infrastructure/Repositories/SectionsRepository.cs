using Courses.Application.RepositoriesContracts;
using Courses.Domain;
using Courses.Domain.Entities;
using Courses.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Courses.Infrastructure.Repositories;

internal sealed class SectionsRepository(CoursesDbContext dbContext) : ISectionsRepository
{
   
    public async Task<Guid> CreateSectionAsync(Section section, CancellationToken cancellationToken = default)
    {
        await dbContext.Sections.AddAsync(section, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return section.Id;
    }

    public async Task<Guid> CreateVideoItemAsync(VideoItem videoItem, CancellationToken cancellationToken = default)
    {
        await dbContext.SectionItems.AddAsync(videoItem, cancellationToken); 

        await dbContext.SaveChangesAsync(cancellationToken);

        return videoItem.Id;
    }
    public async Task<Guid> CreatePdfItemAsync(PdfItem pdfItem, CancellationToken cancellationToken = default)
    {
        await dbContext.SectionItems.AddAsync(pdfItem, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return pdfItem.Id;
    }

    public async Task<int> GetLastDisplayOrderAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IDisplayOrder
    {
        var maxOrder = await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(predicate)
            .Select(x => (int?)x.DisplayOrder)
            .MaxAsync();

        return maxOrder ?? 0;
    }

    public async Task<IList<Section>> GetSectionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await dbContext.Sections
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<SectionItem>> GetSectionItemsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await dbContext.SectionItems
            .Where(i => i.SectionId == sectionId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Section?> GetSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        return await dbContext.Sections
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);
    }

    public async Task<SectionItem?> GetSectionItemByIdAsync(Guid sectionItemId, CancellationToken cancellationToken)
    {
        return await dbContext.SectionItems
            .FirstOrDefaultAsync(i => i.Id == sectionItemId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
