
namespace Commerce.Application.Cart.Features.AddToCart;

public sealed record AddToCartCommand(Guid CourseId) : IRequest<Result>;
