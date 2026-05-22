
namespace Commerce.Application.Cart.Features.ClearCart;

public sealed record ClearCartCommand(Guid StudentId) : IRequest<Result>;
