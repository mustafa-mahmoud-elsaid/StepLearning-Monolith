namespace Commerce.Domain.Orders;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid CourseId { get; private set; }
    public string CourseTitle { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    public static OrderItem Create(Guid orderId, Guid courseId, string courseTitle, decimal price)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            CourseId = courseId,
            CourseTitle = courseTitle,
            Price = price
        };
    }
}
