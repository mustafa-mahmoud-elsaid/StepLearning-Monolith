using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Features.Checkout;

namespace Host.Api.Controllers.Modules.Payments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class PaymentsController(
    IMediator mediator,
    ILogger<PaymentsController> logger) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirst("studentId")?.Value, out var studentId))
            return Unauthorized("Student ID not found in token.");

        var result = await mediator.Send(new CheckoutCommand(studentId, request.CourseId), ct);

        return result.IsSuccess
            ? Ok(new CheckoutResponse(result.Value!))
            : BadRequest(result.Error);
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(ct);
        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

        logger.LogInformation(
            "Stripe webhook triggered. PayloadLength: {PayloadLength}, HasStripeSignature: {HasStripeSignature}",
            payload.Length,
            !string.IsNullOrWhiteSpace(stripeSignature));

        return Ok();
    }
}

public record CheckoutRequest(Guid CourseId);

public record CheckoutResponse(string PaymentUrl);
