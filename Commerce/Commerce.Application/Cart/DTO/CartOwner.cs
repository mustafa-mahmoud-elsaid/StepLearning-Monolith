namespace Commerce.Application.Cart.DTO;

public sealed record CartOwner(string Id, bool IsGuest, Guid? UserId);
