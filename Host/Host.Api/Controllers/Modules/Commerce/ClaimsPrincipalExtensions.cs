using System.Security.Claims;

namespace Host.Api.Controllers.Modules.Commerce;

internal static class ClaimsPrincipalExtensions
{
    public static bool TryGetStudentId(this ClaimsPrincipal user, out Guid studentId) =>
        Guid.TryParse(user.FindFirst("studentId")?.Value, out studentId);
}
