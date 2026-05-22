using Commerce.Domain.Payment;
using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using StepLearning.Shared.Abstraction;

using Commerce.Domain.Orders.Enums;
using Commerce.Application.Orders.Repositories;

namespace Commerce.Application.Payment.Features.Checkout;

internal sealed class Handler(
    IOrderRepository orderRepository,
    IStudentService studentService,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService) : IRequestHandler<CheckoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var validStudent = await studentService.Exists(request.StudentId, cancellationToken);

        if (!validStudent)
            return Result<string>.Failure("Student not found");

        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return Result<string>.Failure("Order not found");

        if (order.StudentId != request.StudentId)
            return Result<string>.Failure("Order does not belong to the student");

        if (order.Status != OrderStatus.Pending)
            return Result<string>.Failure($"Order is not pending. Current status: {order.Status}");

        PaymentRecord payment;
        string paymentUrl;

        try
        {
            payment = PaymentRecord.CreateCheckout(request.StudentId, request.OrderId, order.TotalAmount);
            paymentUrl = await paymentService.CreatePaymentUrl(
                payment.Id,
                payment.StudentId,
                order);
        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Failure(ex.Message);
        }

        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result<string>.Success(paymentUrl);
    }
}
