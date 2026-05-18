using Commerce.Application.Payment.Domain.Entities;
using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Commerce.Application.Payment.Features.Checkout;

internal sealed class Handler(
    ICourseService courseService,
    IStudentService studentService,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService) : IRequestHandler<CheckoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result<string>.Failure("Student id must not be empty");

        if (request.CourseId == Guid.Empty)
            return Result<string>.Failure("Course id must not be empty");

        var validStudent = await studentService.Exists(request.StudentId, cancellationToken);

        if (!validStudent)
            return Result<string>.Failure("Student not found");

        var price = await courseService.GetPrice(request.CourseId, cancellationToken);

        if (!price.HasValue)
            return Result<string>.Failure("Course not found or not available for checkout");

        var hasSucceededPayment = await paymentRepository.HasSucceededPaymentAsync(
            request.StudentId,
            request.CourseId,
            cancellationToken);

        if (hasSucceededPayment)
            return Result<string>.Failure("Student has already paid for this course");

        PaymentRecord payment;
        string paymentUrl;

        try
        {
            payment = PaymentRecord.CreateCheckout(request.StudentId, request.CourseId, price.Value);
            paymentUrl = await paymentService.CreatePaymentUrl(
                payment.Id,
                payment.StudentId,
                payment.CourseId,
                payment.Amount);
        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Failure(ex.Message);
        }

        await paymentRepository.AddAsync(payment, cancellationToken);

        return Result<string>.Success(paymentUrl);
    }
}
