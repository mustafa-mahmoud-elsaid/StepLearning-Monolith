using Commerce.Application.Cart.Features.AddToCart;
using Commerce.Application.Cart.Features.ClearCart;
using Commerce.Application.Cart.Features.GetCart;
using Commerce.Application.Cart.Features.RemoveFromCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Api.Controllers.Modules.Commerce;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Student")]
public sealed class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new AddToCartCommand(request.CourseId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }

    [HttpDelete("items/{courseId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid courseId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveFromCartCommand(courseId), ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCartQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken ct)
    {
        var result = await mediator.Send(new ClearCartCommand(), ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

}

public sealed record AddCartItemRequest(Guid CourseId);
