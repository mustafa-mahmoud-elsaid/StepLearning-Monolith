using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using Courses.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StepLearning.Shared.Pagination;

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

    public async Task<CoursePreviewDto?> GetCoursePreviewAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .Where(c => c.Id == id && c.IsPublished)
            .Select(c => new CoursePreviewDto(
                Id: c.Id,
                Title: c.Title,
                Description: c.Description,
                Thumbnail: c.ThumbnailUrl,
                Price: c.Price,
                Sections: c.Sections
                    .Where(s => s.IsPublished)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new SectionPreviewDto(
                        Id: s.Id,
                        Title: s.Title,
                        SectionItems: s.SectionItems
                            .OrderBy(i => i.DisplayOrder)
                            .Select(i => new SectionItemPreviewDto(
                                Id: i.Id,
                                Title: i.Title,
                                SectionType: i is VideoItem ? "Video" : "Pdf"
                            ))
                    ))
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CourseDetailsDto?> GetCourseDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .Where(c => c.Id == id && c.IsPublished)
            .Select(c => new CourseDetailsDto(
                Id: c.Id,
                Title: c.Title,
                Description: c.Description,
                Thumbnail: c.ThumbnailUrl,
                Sections: c.Sections
                    .Where(s => s.IsPublished)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new SectionDetailsDto(
                        Id: s.Id,
                        Title: s.Title,
                        SectionItems: s.SectionItems
                            .OrderBy(i => i.DisplayOrder)
                            .Select(i => new SectionItemDetailsDto(
                                Id: i.Id,
                                Title: i.Title,
                                ItemType: i is VideoItem ? "Video" : "Pdf",
                                ContentUrl: i is VideoItem
                                    ? ((VideoItem)i).VideoUrl
                                    : ((PdfItem)i).FileUrl,
                                Duration: i is VideoItem
                                    ? (TimeSpan?)((VideoItem)i).Duration
                                    : null
                            ))
                    ))
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResult<CourseCardDto>> GetCourseCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Courses
            .Where(c => c.IsPublished && !c.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var courses = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseCardDto(
                Id: c.Id,
                Title: c.Title,
                Description: c.Description,
                Thumbnail: c.ThumbnailUrl,
                Price: c.Price,
                LastUpdated: c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<CourseCardDto>
        {
            Data = courses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
