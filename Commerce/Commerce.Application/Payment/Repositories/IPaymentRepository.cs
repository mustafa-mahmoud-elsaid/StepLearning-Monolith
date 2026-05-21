using Commerce.Domain.Payment;

namespace Commerce.Application.Payment.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(PaymentRecord payment, CancellationToken ct = default);
    Task<PaymentRecord?> GetByIdAsync(Guid paymentId, CancellationToken ct = default);
    Task<bool> HasSucceededPaymentAsync(Guid studentId, Guid orderId, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
