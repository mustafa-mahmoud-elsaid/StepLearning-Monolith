using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;
using Commerce.Application.Orders.Repositories;
using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Domain.Orders;
using Commerce.Domain.Payment;

namespace Commerce.Application.Payment.Features.Checkout;

internal sealed class Handler(
    ICartCacheRepository cartCacheRepository,
    ICartRepository cartRepository,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService,
    ICartOwnerProvider ownerProvider) : IRequestHandler<CheckoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var owner = ownerProvider.GetOwner();
        var studentId = owner.UserId!.Value;

        // 1. Read cart – cache first, fall back to DB
        var cachedItems = await cartCacheRepository.GetAsync(owner.Key, cancellationToken);

        List<OrderItem> orderItems;

        if (cachedItems is not null && cachedItems.Count > 0)
        {
            orderItems = cachedItems
                .Select(item => OrderItem.Create(Guid.Empty, item.CourseId, item.CourseTitle, item.Price))
                .ToList();
        }
        else
        {
            var cart = await cartRepository.GetByStudentIdAsync(studentId, cancellationToken);

            if (cart is null || !cart.Items.Any())
                return Result<string>.Failure("Cart is empty");

            orderItems = cart.Items
                .Select(item => OrderItem.Create(Guid.Empty, item.CourseId, item.CourseTitle, item.Price))
                .ToList();
        }

        // 2. Create order from cart items
        var order = Order.Create(studentId, orderItems);
        await orderRepository.AddAsync(order, cancellationToken);

        // 3. Create payment record + Stripe session
        PaymentRecord payment;
        string paymentUrl;

        try
        {
            payment = PaymentRecord.CreateCheckout(studentId, order.Id, order.TotalAmount);
            paymentUrl = await paymentService.CreatePaymentUrl(payment.Id, studentId, order);
        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Failure(ex.Message);
        }

        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result<string>.Success(paymentUrl);
    }
}
