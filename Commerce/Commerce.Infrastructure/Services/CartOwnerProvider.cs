using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.ServicesInterfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Commerce.Infrastructure.Services;

internal sealed class CartOwnerProvider(IHttpContextAccessor httpContextAccessor) : ICartOwnerProvider
{
    public CartOwner GetOwnerId()
    {
        var context = httpContextAccessor.HttpContext;

        var user = context.User;

        if (user.Identity?.IsAuthenticated is true)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            return new($"user:{userId}", false, Guid.TryParse(userId, out var result) ? result : Guid.Empty);
        }

        const string cookieName = "guest-cart-id";

        if (!context.Request.Cookies.TryGetValue(cookieName, out var guestId))
        {
            guestId = Guid.NewGuid().ToString();

            context.Response.Cookies.Append(cookieName, guestId);
        }

        return new($"guest:${guestId}", false, null);
    }
}
