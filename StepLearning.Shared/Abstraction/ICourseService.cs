namespace StepLearning.Shared.Abstraction;

public interface ICourseService
{
    Task<bool> Exists(Guid courseId, CancellationToken ct = default);
    Task<decimal?> GetPrice(Guid courseId, CancellationToken ct = default);
    Task<CourseSnapshot?> GetSnapshot(Guid courseId, CancellationToken ct = default);
}

public sealed record CourseSnapshot(Guid CourseId, string Title, decimal Price, string? ThumbnailUrl);
