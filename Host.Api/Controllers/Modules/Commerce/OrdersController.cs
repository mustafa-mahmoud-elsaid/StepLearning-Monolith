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
    private Guid GetStudentId()
    {
        var studentIdClaim = User.FindFirst("studentId");
        if (studentIdClaim is null || !Guid.TryParse(studentIdClaim.Value, out var studentId))
            throw new UnauthorizedAccessException("Student id not found in token");
        return studentId;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(GetStudentId());
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { Error = result.Error });

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, new { OrderId = result.Value });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderQuery(id, GetStudentId());
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { Error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderHistory(CancellationToken cancellationToken)
    {
        var query = new GetOrderHistoryQuery(GetStudentId());
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Value);
    }
}
