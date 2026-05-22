using Identity.Domain.Entities;

namespace Identity.Application.RepositoryInterfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
