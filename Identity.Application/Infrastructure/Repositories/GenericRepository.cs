using Identity.Application.Infrastructure.Data;
using Identity.Application.RepositoryInterfaces;

namespace Identity.Application.Infrastructure.Repositories;

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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
