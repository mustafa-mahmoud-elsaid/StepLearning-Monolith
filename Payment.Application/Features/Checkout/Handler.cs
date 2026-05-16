using MassTransit;
using MediatR;
using Payment.Application.Domain.Entities;
using Payment.Application.Repositories;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;
using StepLearning.Shared.Result;

namespace Payment.Application.Features.Checkout;

internal sealed class Handler(
    ICourseService courseService,
    IStudentService studentService,
    IPaymentRepository paymentRepository) : IRequestHandler<CheckoutCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result<Guid>.Failure("Student id must not be empty");

        if (request.CourseId == Guid.Empty)
            return Result<Guid>.Failure("Course id must not be empty");

        var validStudent = await studentService.Exists(request.StudentId, cancellationToken);

        if (!validStudent)
            return Result<Guid>.Failure("Student not found");

        var price = await courseService.GetPrice(request.CourseId, cancellationToken);

        if (!price.HasValue)
            return Result<Guid>.Failure("Course not found or not available for checkout");

        var hasSucceededPayment = await paymentRepository.HasSucceededPaymentAsync(
            request.StudentId,
            request.CourseId,
            cancellationToken);

        if (hasSucceededPayment)
            return Result<Guid>.Failure("Student has already paid for this course");

        PaymentRecord payment;

        try
        {
            payment = PaymentRecord.CreateCheckout(request.StudentId, request.CourseId, price.Value);
            
            // FAKE PAYMENT INTEGRATION (To be replaced with real Stripe/Paymob later)
            payment.MarkSucceeded($"fake_txn_{Guid.NewGuid().ToString("N")[..8]}");
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await paymentRepository.AddAsync(payment, cancellationToken);

        // TODO: Publish event to RabbitMQ so Enrollment module can consume it

        return Result<Guid>.Success(payment.Id);
    }
}
