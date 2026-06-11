
namespace Commerce.Application.Payment.Features.Checkout;

public record CheckoutCommand() : IRequest<Result<string>>;
