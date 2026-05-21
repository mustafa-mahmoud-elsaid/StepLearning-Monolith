using Commerce.Application.Cart.Repositories;
using Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CartEntity = Commerce.Domain.Cart.Cart;

namespace Commerce.Infrastructure.Repositories;

internal sealed class CartRepository(CommerceDbContext dbContext) : ICartRepository
{
    public async Task<CartEntity?> GetByStudentIdAsync(Guid studentId, CancellationToken ct = default)
    {
        return await dbContext.Carts
            .Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.StudentId == studentId, ct);
    }

    public async Task AddAsync(CartEntity cart, CancellationToken ct = default)
    {
        await dbContext.Carts.AddAsync(cart, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
