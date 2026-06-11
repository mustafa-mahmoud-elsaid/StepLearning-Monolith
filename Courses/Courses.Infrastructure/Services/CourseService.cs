using Courses.Application.RepositoriesContracts;
using Microsoft.Extensions.Caching.Distributed;
using StepLearning.Shared.Abstraction;
using System.Text.Json;

namespace Courses.Infrastructure.Services;

internal class CourseService(ICoursesRepository coursesRepository, IDistributedCache cache) : ICourseService
{
    public async Task<bool> Exists(Guid courseId, CancellationToken ct = default)
    {
        if(courseId == Guid.Empty) 
            return false;

        return await coursesRepository.Exists(courseId, ct);
    }

    public async Task<decimal?> GetPrice(Guid courseId, CancellationToken ct = default)
    {
        if (courseId == Guid.Empty)
            return null;

        return await coursesRepository.GetCoursePriceAsync(courseId, ct);
    }

    public async Task<CourseSnapshot?> GetSnapshot(Guid courseId, CancellationToken ct = default)
    {
        if (courseId == Guid.Empty)
            return null;

        var key = $"course.snapshot.{courseId}";

        var cachedResult = await cache.GetStringAsync(key, ct);

        if (!string.IsNullOrEmpty(cachedResult))
            return JsonSerializer.Deserialize<CourseSnapshot>(cachedResult);

        var courseSnapshot = await coursesRepository.GetCourseSnapshotAsync(courseId, ct);

        if (courseSnapshot is null)
            return null;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
        };

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(courseSnapshot),
            options,
            ct);

        return courseSnapshot;

    }
}
