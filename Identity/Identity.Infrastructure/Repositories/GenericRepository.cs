using Identity.Infrastructure.Data;
using Identity.Application.RepositoryInterfaces;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Repositories;

internal class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly UsersDbContext _dbContext;

    public GenericRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _dbContext.Set<T>().AnyAsync(predicate, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
