using System.Security.Claims;

namespace Identity.Application.Interfaces;

public interface IUserClaimsProvider
{
    Task<IEnumerable<Claim>> GetAdditionalClaimsAsync(Guid userId, CancellationToken ct = default);
}
