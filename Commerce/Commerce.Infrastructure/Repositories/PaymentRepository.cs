using Commerce.Domain.Payment;
using Commerce.Domain.Payment.Enums;
using Commerce.Application.Payment.Repositories;
using Commerce.Infrastructure.Data;

namespace Commerce.Infrastructure.Repositories;

internal sealed class PaymentRepository(CommerceDbContext dbContext) : IPaymentRepository
{
    public async Task AddAsync(PaymentRecord payment, CancellationToken ct = default)
    {
        await dbContext.PaymentRecords.AddAsync(payment, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<PaymentRecord?> GetByIdAsync(Guid paymentId, CancellationToken ct = default)
    {
        return await dbContext.PaymentRecords
            .FirstOrDefaultAsync(payment => payment.Id == paymentId, ct);
    }

    public async Task<bool> HasSucceededPaymentAsync(Guid studentId, Guid orderId, CancellationToken ct = default)
    {
        return await dbContext.PaymentRecords
            .AsNoTracking()
            .AnyAsync(
                payment => payment.StudentId == studentId
                    && payment.OrderId == orderId
                    && payment.Status == PaymentStatus.Succeeded,
                ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
