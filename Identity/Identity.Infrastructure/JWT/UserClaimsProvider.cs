using System.Security.Claims;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.JWT;

internal sealed class UserClaimsProvider(UsersDbContext dbContext) : IUserClaimsProvider
{
    public async Task<IEnumerable<Claim>> GetAdditionalClaimsAsync(Guid userId, CancellationToken ct = default)
    {
        var claims = new List<Claim>();

        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.UserId == userId, ct);
        if (student is not null)
            claims.Add(new Claim("studentId", student.Id.ToString()));

        var instructor = await dbContext.Instructors.FirstOrDefaultAsync(i => i.UserId == userId, ct);
        if (instructor is not null)
            claims.Add(new Claim("instructorId", instructor.Id.ToString()));

        return claims;
    }
}
