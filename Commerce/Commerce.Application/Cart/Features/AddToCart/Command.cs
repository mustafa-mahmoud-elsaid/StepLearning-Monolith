
namespace Commerce.Application.Cart.Features.AddToCart;

public sealed record AddToCartCommand(Guid StudentId, Guid CourseId) : IRequest<Result>;
