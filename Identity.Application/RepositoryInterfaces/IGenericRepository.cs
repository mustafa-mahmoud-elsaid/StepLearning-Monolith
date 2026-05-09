namespace Identity.Application.RepositoryInterfaces;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Mark entity as added; requires SaveChanges to persist.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
