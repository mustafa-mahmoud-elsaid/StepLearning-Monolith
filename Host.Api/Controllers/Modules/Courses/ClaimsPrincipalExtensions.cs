using System.Security.Claims;

namespace Host.Api.Controllers.Modules.Courses;

internal static class ClaimsPrincipalExtensions
{
    public static bool TryGetStudentId(this ClaimsPrincipal user, out Guid studentId) =>
        Guid.TryParse(user.FindFirst("studentId")?.Value, out studentId);

    public static bool TryGetInstructorId(this ClaimsPrincipal user, out Guid instructorId) =>
        Guid.TryParse(user.FindFirst("instructorId")?.Value, out instructorId);
}
