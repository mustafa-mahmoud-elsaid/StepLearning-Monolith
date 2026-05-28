namespace Commerce.Application.Cart.DTO;

public sealed record CartOwner(string Key, bool IsGuest, Guid? UserId);
