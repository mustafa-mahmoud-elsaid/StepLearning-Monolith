using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Models;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;

namespace Notifications.Infrastructure.Consumers;

public sealed class EnrollmentCompletedNotificationConsumer(
    IEmailSender emailSender,
    ILogger<EnrollmentCompletedNotificationConsumer> logger) : IConsumer<EnrollmentCompletedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentCompletedEvent> context)
    {
        var message = context.Message;
        var email = message.StudentEmail;

        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning("Skipping enrollment email because no email was found in the event");
            return;
        }

        var courseNames = string.Join(", ", message.Courses.Select(c => c.CourseName));

        var emailMessage = new EmailMessage(
            email,
            "Enrollment confirmed",
            $"Your enrollment has been completed successfully for courses: {courseNames}.");

        await emailSender.SendAsync(emailMessage, context.CancellationToken);
    }
}
