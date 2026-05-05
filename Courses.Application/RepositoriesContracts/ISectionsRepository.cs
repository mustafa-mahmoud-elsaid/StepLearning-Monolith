using Courses.Domain.Entities;

namespace Courses.Application.RepositoriesContracts;

public interface ISectionsRepository
{
    Task<Guid> CreateSectionAsync(Section section, CancellationToken cancellationToken = default);

    /// <summary>
    /// get last section order in a specific course
    /// </summary>
    /// <param name="courseId"></param>
    /// <returns>0 if it is the first section to add | last secion order </returns>
    Task<int> GetLastDisplayOrderAsync(Guid courseId);
}
