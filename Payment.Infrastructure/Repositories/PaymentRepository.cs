using Payment.Application.Domain.Entities;
using Payment.Application.Repositories;
using Payment.Infrastructure.Data;

namespace Payment.Infrastructure.Repositories;

internal sealed class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
{
    public async Task AddAsync(PaymentRecord payment, CancellationToken ct = default)
    {
        await dbContext.PaymentRecords.AddAsync(payment, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}
