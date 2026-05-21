using Enrollment.Application.Domain.Enums;
using Enrollment.Application.DTO;
using Enrollment.Application.Features.Commands.Enroll;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using StepLearning.Shared.Events;

namespace Enrollment.Application.Consumers;

public sealed class PaymentSucceededConsumer(
    ISender sender,
    ILogger<PaymentSucceededConsumer> logger) : IConsumer<PaymentSucceededEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var message = context.Message;

        var command = new EnrollStudentCommand(
            new EnrollStudentRequestDto(
                message.StudentId,
                message.CourseIds,
                message.PaymentId,
                EnrollmentStatus.Active));

        var result = await sender.Send(command, context.CancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Payment succeeded event was consumed but enrollment was not created. PaymentId: {PaymentId}, StudentId: {StudentId}, Reason: {Reason}",
                message.PaymentId,
                message.StudentId,
                result.Error);
        }
    }
}
