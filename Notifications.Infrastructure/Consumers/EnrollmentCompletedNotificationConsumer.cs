using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Models;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;

namespace Notifications.Infrastructure.Consumers;

public sealed class EnrollmentCompletedNotificationConsumer(
    IStudentService studentService,
    IEmailSender emailSender,
    ILogger<EnrollmentCompletedNotificationConsumer> logger) : IConsumer<EnrollmentCompletedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentCompletedEvent> context)
    {
        var message = context.Message;
        var email = await studentService.GetEmail(message.StudentId, context.CancellationToken);

        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning(
                "Skipping enrollment email because no email was found for student {StudentId}",
                message.StudentId);
            return;
        }

        var emailMessage = new EmailMessage(
            email,
            "Enrollment confirmed",
            $"Your enrollment has been completed successfully for course {message.CourseId}.");

        await emailSender.SendAsync(emailMessage, context.CancellationToken);
    }
}
