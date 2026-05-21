using Commerce.Application.Orders.Domain.Entities;

namespace Commerce.Application.Payment.ServicesInterfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentUrl(Guid paymentId, Guid userId, Order order);
}
