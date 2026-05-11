using Identity.Application.Domain.Entities;
using Identity.Application.Infrastructure.Data;
using Identity.Application.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Infrastructure.Repositories;

internal class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UsersDbContext _dbContext;

    public RefreshTokenRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(entity, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
