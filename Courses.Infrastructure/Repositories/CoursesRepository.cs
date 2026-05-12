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

    public async Task<PaginatedResult<InstructorCourseDto>> GetInstructorCoursesAsync(Guid instructorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Courses
            .Where(c => c.InstructorId == instructorId && !c.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new InstructorCourseDto(
                Id: c.Id,
                Title: c.Title,
                Description: c.Description,
                Thumbnail: c.ThumbnailUrl,
                Price: c.Price,
                IsPublished: c.IsPublished,
                CreatedAt: c.CreatedAt,
                LastUpdated: c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<InstructorCourseDto>
        {
            Data = courses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedResult<CourseCardDto>> SearchCoursesAsync(
        string? title, decimal? minPrice, decimal? maxPrice,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Courses
            .Where(c => c.IsPublished && !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(c => EF.Functions.Like(c.Title, $"%{title}%"));

        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

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

    public async Task<Course?> GetCourseWithSectionsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .AsNoTracking()
            .Include(c => c.Sections)
                .ThenInclude(s => s.SectionItems)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<Course?> GetCourseByIdEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> GetInstructorId(Guid courseId, CancellationToken cancellationToken = default)
    {
        if (courseId == Guid.Empty)
            throw new InvalidOperationException("course id can not be empty");
        return await _dbContext.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .Select(c => c.InstructorId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> Exists(Guid courseId, CancellationToken ct = default) 
        => await _dbContext.Courses.AnyAsync(c => c.Id == courseId &&
        c.IsPublished &&
        !c.IsDeleted, ct);
}
