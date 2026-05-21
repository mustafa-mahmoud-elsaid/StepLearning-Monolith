namespace Commerce.Application.Orders.DTO;

public record OrderDto(
    Guid Id,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    List<OrderItemDto> Items);
