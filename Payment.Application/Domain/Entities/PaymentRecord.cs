namespace Payment.Application.Domain.Entities;

public class PaymentRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string StripeChargeId { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
