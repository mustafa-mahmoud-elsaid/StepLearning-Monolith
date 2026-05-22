using Commerce.Domain.Orders;
using Commerce.Application.Orders.Repositories;
using Commerce.Infrastructure.Data;

namespace Commerce.Infrastructure.Repositories;

internal sealed class OrderRepository(CommerceDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<Order>> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.StudentId == studentId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await dbContext.Orders.AddAsync(order, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
