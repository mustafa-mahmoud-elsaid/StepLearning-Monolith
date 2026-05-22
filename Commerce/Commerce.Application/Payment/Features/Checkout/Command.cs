
namespace Commerce.Application.Payment.Features.Checkout;

public record CheckoutCommand(Guid StudentId, Guid OrderId) : IRequest<Result<string>>;
