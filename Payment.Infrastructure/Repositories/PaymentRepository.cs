using Payment.Application.Domain.Entities;
using Payment.Application.Domain.Enums;
using Payment.Application.Repositories;
using Payment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Repositories;

internal sealed class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
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

    public async Task<bool> HasSucceededPaymentAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        return await dbContext.PaymentRecords
            .AsNoTracking()
            .AnyAsync(
                payment => payment.StudentId == studentId
                    && payment.CourseId == courseId
                    && payment.Status == PaymentStatus.Succeeded,
                ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
