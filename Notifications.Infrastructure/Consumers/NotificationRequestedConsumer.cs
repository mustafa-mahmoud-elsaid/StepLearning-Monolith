using MassTransit;
using Notifications.Application.Abstractions;
using Notifications.Application.Models;
using StepLearning.Shared.Events;

namespace Notifications.Infrastructure.Consumers;

public sealed class NotificationRequestedConsumer(IEmailSender emailSender) : IConsumer<NotificationRequestedEvent>
{
    private const string EmailChannel = "Email";

    public async Task Consume(ConsumeContext<NotificationRequestedEvent> context)
    {
        var message = context.Message;

        if (!string.Equals(message.Channel, EmailChannel, StringComparison.OrdinalIgnoreCase))
            return;

        var emailMessage = new EmailMessage(
            message.To,
            message.Subject,
            BuildBody(message.TemplateKey, message.Data),
            message.TemplateKey,
            message.Data);

        await emailSender.SendAsync(emailMessage, context.CancellationToken);
    }

    private static string BuildBody(string templateKey, IReadOnlyDictionary<string, string> data)
    {
        if (string.Equals(templateKey, "Enrollment.Completed", StringComparison.OrdinalIgnoreCase))
        {
            var courseId = data.GetValueOrDefault("CourseId", "your course");
            return $"Your enrollment has been completed successfully for course {courseId}.";
        }

        return data.GetValueOrDefault("Body", "You have a new notification.");
    }
}
