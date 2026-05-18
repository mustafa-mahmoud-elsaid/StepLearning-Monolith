namespace Commerce.Application.Payment.ServicesInterfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentUrl(Guid paymentId, Guid userId, Guid courseId, decimal amount);
}
