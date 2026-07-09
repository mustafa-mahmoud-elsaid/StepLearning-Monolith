using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;
using Commerce.Application.Orders.Features.CreateOrder;
using Commerce.Application.Orders.Repositories;
using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Domain.Payment;

namespace Commerce.Application.Payment.Features.Checkout;

internal sealed class Handler(
    ICartCacheRepository cartCacheRepository,
    ICartRepository cartRepository,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService,
    ICartOwnerProvider ownerProvider,
    ISender sender) : IRequestHandler<CheckoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var owner = ownerProvider.GetOwner();
        var studentId = owner.UserId!.Value;

        // 1. Sync cache → DB so CreateOrderCommand sees the latest items
        var cachedItems = await cartCacheRepository.GetAsync(owner.Key, cancellationToken);

        if (cachedItems is not null && cachedItems.Count > 0)
        {
            var cart = await cartRepository.GetByStudentIdAsync(studentId, cancellationToken);
            var isNewCart = cart is null;

            cart ??= Domain.Cart.Cart.Create(studentId);
            cart.Clear();

            foreach (var item in cachedItems)
            {
                cart.AddItem(item.CourseId, item.Price, item.CourseTitle);
            }

            if (isNewCart)
                await cartRepository.AddAsync(cart, cancellationToken);

            await cartRepository.SaveChangesAsync(cancellationToken);
        }

        // 2. Create order from cart (also clears the DB cart)
        var orderResult = await sender.Send(new CreateOrderCommand(studentId), cancellationToken);

        if (!orderResult.IsSuccess)
            return Result<string>.Failure(orderResult.Error!);

        // 3. Clear the cache cart
        await cartCacheRepository.RemoveCartAsync(owner.Key);

        // 4. Create payment record + Stripe session
        var order = await orderRepository.GetByIdAsync(orderResult.Value, cancellationToken);

        PaymentRecord payment;
        string paymentUrl;

        try
        {
            payment = PaymentRecord.CreateCheckout(studentId, order!.Id, order.TotalAmount);
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
