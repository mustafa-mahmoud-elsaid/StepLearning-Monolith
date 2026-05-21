using System.Security.Claims;

namespace Identity.Application.Infrastructure.JWT;

public interface IUserClaimsProvider
{
    Task<IEnumerable<Claim>> GetAdditionalClaimsAsync(string userId, CancellationToken ct = default);
}
