namespace Payment.Application.ServicesInterfaces;

public interface IPaymentWebhookService
{
    Task<PaymentWebhookResult> HandleStripeWebhookAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default);
}

public sealed record PaymentWebhookResult(PaymentWebhookStatus Status, string? Error = null)
{
    public static PaymentWebhookResult Processed() => new(PaymentWebhookStatus.Processed);
    public static PaymentWebhookResult Ignored() => new(PaymentWebhookStatus.Ignored);
    public static PaymentWebhookResult Invalid(string error) => new(PaymentWebhookStatus.Invalid, error);
    public static PaymentWebhookResult NotFound(string error) => new(PaymentWebhookStatus.NotFound, error);
}

public enum PaymentWebhookStatus
{
    Processed,
    Ignored,
    Invalid,
    NotFound
}
