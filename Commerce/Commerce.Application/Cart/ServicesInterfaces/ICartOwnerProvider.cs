using Commerce.Application.Cart.DTO;

namespace Commerce.Application.Cart.ServicesInterfaces;

public interface ICartOwnerProvider
{
    CartOwner GetOwnerId();
}
