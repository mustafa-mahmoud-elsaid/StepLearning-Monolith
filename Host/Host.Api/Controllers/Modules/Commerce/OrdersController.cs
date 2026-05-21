using Commerce.Application.Orders.Features.CreateOrder;
using Commerce.Application.Orders.Features.GetOrder;
using Commerce.Application.Orders.Features.GetOrderHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Host.Api.Controllers.Modules.Commerce;

[Route("api/orders")]
[ApiController]
[Authorize(Roles = "Student")]
public class OrdersController(IMediator mediator) : ControllerBase
{


    [HttpPost]
    public async Task<IActionResult> CreateOrder(CancellationToken cancellationToken)
    {
        if (!User.TryGetStudentId(out var studentId))
            return Unauthorized("Student id not found in token");

        var command = new CreateOrderCommand(studentId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { Error = result.Error });

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, new { OrderId = result.Value });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        if (!User.TryGetStudentId(out var studentId))
            return Unauthorized("Student id not found in token");

        var query = new GetOrderQuery(id, studentId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { Error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderHistory(CancellationToken cancellationToken)
    {
        if (!User.TryGetStudentId(out var studentId))
            return Unauthorized("Student id not found in token");

        var query = new GetOrderHistoryQuery(studentId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Value);
    }
}
