using Commerce.Application.Payment.Domain.Enums;

namespace Commerce.Application.Payment.Domain.Entities;

public class PaymentRecord
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? ProviderReference { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public static PaymentRecord CreateCheckout(Guid studentId, Guid orderId, decimal amount, string currency = "USD")
    {
        if (studentId == Guid.Empty)
            throw new InvalidOperationException("Student id can not be empty");

        if (orderId == Guid.Empty)
            throw new InvalidOperationException("Order id can not be empty");

        if (amount < 0)
            throw new InvalidOperationException("Payment amount can not be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new InvalidOperationException("Currency is required");

        return new()
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            OrderId = orderId,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkSucceeded(string providerReference)
    {
        if (Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException("Payment already succeeded");

        if (string.IsNullOrWhiteSpace(providerReference))
            throw new InvalidOperationException("Provider reference is required");

        ProviderReference = providerReference;
        Status = PaymentStatus.Succeeded;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        if (Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException("Succeeded payment can not be failed");

        Status = PaymentStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}
