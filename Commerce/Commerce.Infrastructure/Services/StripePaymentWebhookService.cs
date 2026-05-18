using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;
using Stripe;
using Stripe.Checkout;

namespace Commerce.Infrastructure.Services;

internal sealed class StripePaymentWebhookService(
    IOptions<StripeOptions> stripeOptions,
    IPaymentRepository paymentRepository,
    IIntegrationEventPublisher integrationEventPublisher,
    ILogger<StripePaymentWebhookService> logger) : IPaymentWebhookService
{
    private const string CheckoutSessionCompleted = "checkout.session.completed";

    public async Task<PaymentWebhookResult> HandleStripeWebhookAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Stripe webhook triggered. PayloadLength: {PayloadLength}, HasStripeSignature: {HasStripeSignature}",
            payload.Length,
            !string.IsNullOrWhiteSpace(signature));

        var webhookSecret = stripeOptions.Value.WebhookSecret;

        if (string.IsNullOrWhiteSpace(webhookSecret))
            return PaymentWebhookResult.Invalid("Stripe webhook secret is not configured.");

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe webhook signature verification failed.");
            return PaymentWebhookResult.Invalid("Invalid Stripe webhook signature.");
        }

        if (stripeEvent.Type != CheckoutSessionCompleted)
        {
            logger.LogInformation("Stripe webhook ignored. EventType: {EventType}", stripeEvent.Type);
            return PaymentWebhookResult.Ignored();
        }

        if (stripeEvent.Data.Object is not Session session)
            return PaymentWebhookResult.Invalid("Unexpected Stripe checkout session payload.");

        if (!session.Metadata.TryGetValue("paymentId", out var paymentIdValue)
            || !Guid.TryParse(paymentIdValue, out var paymentId))
        {
            return PaymentWebhookResult.Invalid("Stripe checkout session did not include a valid paymentId.");
        }

        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);

        if (payment is null)
            return PaymentWebhookResult.NotFound($"Payment {paymentId} was not found.");

        try
        {
            payment.MarkSucceeded(session.PaymentIntentId ?? session.Id);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogInformation(ex, "Payment {PaymentId} was already processed.", paymentId);
            return PaymentWebhookResult.Processed();
        }

        await paymentRepository.SaveChangesAsync(cancellationToken);

        await integrationEventPublisher.PublishAsync(
            new PaymentSucceededEvent(payment.StudentId, payment.CourseId, payment.Id),
            cancellationToken);

        logger.LogInformation(
            "Payment {PaymentId} succeeded from Stripe webhook and PaymentSucceededEvent was published.",
            payment.Id);

        return PaymentWebhookResult.Processed();
    }
}
