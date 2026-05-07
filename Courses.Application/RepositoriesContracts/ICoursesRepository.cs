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
}
