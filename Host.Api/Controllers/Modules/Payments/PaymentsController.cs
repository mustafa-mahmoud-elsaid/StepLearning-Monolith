using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Commerce.Application.Payment.Features.Checkout;
using Commerce.Application.Payment.ServicesInterfaces;

namespace Host.Api.Controllers.Modules.Payments;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class PaymentsController(
    IMediator mediator,
    IPaymentWebhookService paymentWebhookService) : ControllerBase
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
        var result = await paymentWebhookService.HandleStripeWebhookAsync(payload, stripeSignature, ct);

        return result.Status switch
        {
            PaymentWebhookStatus.Processed => Ok(),
            PaymentWebhookStatus.Ignored => Ok(),
            PaymentWebhookStatus.NotFound => NotFound(result.Error),
            PaymentWebhookStatus.Invalid => BadRequest(result.Error),
            _ => BadRequest()
        };
    }
}

public record CheckoutRequest(Guid CourseId);

public record CheckoutResponse(string PaymentUrl);
