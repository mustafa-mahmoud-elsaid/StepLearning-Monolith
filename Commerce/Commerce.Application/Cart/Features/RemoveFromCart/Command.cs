
namespace Commerce.Application.Cart.Features.RemoveFromCart;

public sealed record RemoveFromCartCommand(Guid CourseId) : IRequest<Result>;
