using System.Security.Claims;

namespace Identity.Application.Infrastructure.JWT;

public interface IUserClaimsProvider
{
    Task<IEnumerable<Claim>> GetAdditionalClaimsAsync(Guid userId, CancellationToken ct = default);
}
