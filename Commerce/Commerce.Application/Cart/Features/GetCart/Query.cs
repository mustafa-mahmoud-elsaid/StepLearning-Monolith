using Commerce.Application.Cart.DTO;

namespace Commerce.Application.Cart.Features.GetCart;

public sealed record GetCartQuery(Guid StudentId) : IRequest<Result<CartDto>>;
