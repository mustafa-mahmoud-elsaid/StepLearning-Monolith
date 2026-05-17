using Payment.Application.Domain.Entities;

namespace Payment.Application.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(PaymentRecord payment, CancellationToken ct = default);
    Task<bool> HasSucceededPaymentAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
}
