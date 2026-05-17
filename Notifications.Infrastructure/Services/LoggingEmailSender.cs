using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Models;

namespace Notifications.Infrastructure.Services;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email notification prepared. To: {To}, Subject: {Subject}, Body: {Body}",
            message.To,
            message.Subject,
            message.Body);

        return Task.CompletedTask;
    }
}
