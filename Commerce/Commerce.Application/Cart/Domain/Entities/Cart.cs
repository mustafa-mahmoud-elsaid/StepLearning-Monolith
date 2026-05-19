namespace Commerce.Application.Cart.Domain.Entities;

public class Cart
{
    private readonly List<CartItem> _items = [];

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public ICollection<CartItem> Items => _items;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Cart Create(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new InvalidOperationException("Student id must not be empty");

        var now = DateTimeOffset.UtcNow;

        return new()
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void AddItem(Guid courseId, decimal price, string courseTitle)
    {
        if (_items.Any(item => item.CourseId == courseId))
            throw new InvalidOperationException("Course is already in cart");

        _items.Add(CartItem.Create(Id, courseId, price, courseTitle));
        Touch();
    }

    public void RemoveItem(Guid courseId)
    {
        var item = _items.FirstOrDefault(cartItem => cartItem.CourseId == courseId);

        if (item is null)
            throw new InvalidOperationException("Course is not in cart");

        _items.Remove(item);
        Touch();
    }

    public void Clear()
    {
        _items.Clear();
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
