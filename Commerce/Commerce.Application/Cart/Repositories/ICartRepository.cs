using CartEntity = Commerce.Application.Cart.Domain.Entities.Cart;

namespace Commerce.Application.Cart.Repositories;

public interface ICartRepository
{
    Task<CartEntity?> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default);
    Task AddAsync(CartEntity cart, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
