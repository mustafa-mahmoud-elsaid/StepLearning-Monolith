using Courses.Domain;
using Courses.Domain.Entities;
using System.Linq.Expressions;

namespace Courses.Application.RepositoriesContracts;

public interface ISectionsRepository
{
    Task<Guid> CreateSectionAsync(Section section, CancellationToken cancellationToken = default);

    /// <summary>
    /// get last section order in a specific course
    /// </summary>
    /// <param name="courseId"></param>
    /// <returns>0 if it is the first section to add | last secion order </returns>
    Task<int> GetLastDisplayOrderAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IDisplayOrder;

    Task<Guid> CreateVideoItemAsync(VideoItem videoItem, CancellationToken cancellationToken = default);
    Task<Guid> CreatePdfItemAsync(PdfItem pdfItem, CancellationToken cancellationToken = default);
    Task<IList<Section>> GetSectionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IList<SectionItem>> GetSectionItemsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<Section?> GetSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken = default);
    Task<SectionItem?> GetSectionItemByIdAsync(Guid sectionItemId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

