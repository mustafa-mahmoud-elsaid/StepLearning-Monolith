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
        if (!TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await mediator.Send(new AddToCartCommand(studentId, request.CourseId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }

    [HttpDelete("items/{courseId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid courseId, CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await mediator.Send(new RemoveFromCartCommand(studentId, courseId), ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await mediator.Send(new GetCartQuery(studentId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await mediator.Send(new ClearCartCommand(studentId), ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

    private bool TryGetStudentId(out Guid studentId)
    {
        return Guid.TryParse(User.FindFirst("studentId")?.Value, out studentId);
    }
}

public sealed record AddCartItemRequest(Guid CourseId);
