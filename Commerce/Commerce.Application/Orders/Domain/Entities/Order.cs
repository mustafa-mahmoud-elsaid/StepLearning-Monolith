using Commerce.Application.Orders.Domain.Enums;

namespace Commerce.Application.Orders.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ICollection<OrderItem> Items => _items;
    public Guid? PaymentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Order Create(Guid studentId, IEnumerable<OrderItem> items)
    {
        if (studentId == Guid.Empty)
            throw new InvalidOperationException("Student id must not be empty");

        var orderItems = items.ToList();
        if (!orderItems.Any())
            throw new InvalidOperationException("Order must have at least one item");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            TotalAmount = orderItems.Sum(x => x.Price)
        };

        foreach (var item in orderItems)
        {
            order._items.Add(item);
        }

        return order;
    }

    public void MarkAsPaid(Guid paymentId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be marked as paid");

        Status = OrderStatus.Paid;
        PaymentId = paymentId;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be cancelled");

        Status = OrderStatus.Cancelled;
    }
}
