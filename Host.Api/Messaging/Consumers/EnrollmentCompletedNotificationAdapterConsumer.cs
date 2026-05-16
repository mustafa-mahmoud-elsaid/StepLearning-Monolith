using MassTransit;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Events;

namespace Host.Api.Messaging.Consumers;

public sealed class EnrollmentCompletedNotificationAdapterConsumer(
    IStudentService studentService,
    IIntegrationEventPublisher integrationEventPublisher,
    ILogger<EnrollmentCompletedNotificationAdapterConsumer> logger) : IConsumer<EnrollmentCompletedEvent>
{
    public async Task Consume(ConsumeContext<EnrollmentCompletedEvent> context)
    {
        var message = context.Message;
        var email = await studentService.GetEmail(message.StudentId, context.CancellationToken);

        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning(
                "Skipping enrollment notification because no email was found for student {StudentId}",
                message.StudentId);
            return;
        }

        var notification = new NotificationRequestedEvent(
            Guid.NewGuid(),
            "Email",
            "Enrollment.Completed",
            email,
            "Enrollment confirmed",
            new Dictionary<string, string>
            {
                ["EnrollmentId"] = message.EnrollmentId.ToString(),
                ["StudentId"] = message.StudentId.ToString(),
                ["CourseId"] = message.CourseId.ToString(),
                ["PaymentId"] = message.PaymentId.ToString()
            },
            message.EnrollmentId,
            DateTime.UtcNow);

        await integrationEventPublisher.PublishAsync(notification, context.CancellationToken);
    }
}
