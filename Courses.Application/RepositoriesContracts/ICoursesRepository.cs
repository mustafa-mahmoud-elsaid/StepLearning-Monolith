using Courses.Application.DTO;
using Courses.Domain.Entities;
using StepLearning.Shared.Pagination;

namespace Courses.Application.RepositoriesContracts;

public interface ICoursesRepository
{
    Task<Guid> CreateCourseAsync(Course course, CancellationToken cancellationToken = default);
    Task<CoursePreviewDto?> GetCoursePreviewAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CourseDetailsDto?> GetCourseDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<CourseCardDto>> GetCourseCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedResult<InstructorCourseDto>> GetInstructorCoursesAsync(Guid instructorId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedResult<CourseCardDto>> SearchCoursesAsync(string? title, decimal? minPrice, decimal? maxPrice, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseWithSectionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseByIdEntityAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Guid> GetInstructorId(Guid courseId,  CancellationToken cancellationToken = default);
    Task<PaginatedResult<CourseCardDto>> GetStudentCoursesAsync(IEnumerable<Guid> coursesIds, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid courseId, CancellationToken ct = default);
    Task<decimal?> GetCoursePriceAsync(Guid courseId, CancellationToken ct = default);
}
