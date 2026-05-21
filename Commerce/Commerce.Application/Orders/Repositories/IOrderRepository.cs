using Commerce.Domain.Orders;

namespace Commerce.Application.Orders.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Order>> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
