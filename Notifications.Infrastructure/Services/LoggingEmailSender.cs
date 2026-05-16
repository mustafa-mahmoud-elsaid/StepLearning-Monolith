using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Models;

namespace Notifications.Infrastructure.Services;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email notification prepared. To: {To}, Subject: {Subject}, TemplateKey: {TemplateKey}, Data: {@Data}",
            message.To,
            message.Subject,
            message.TemplateKey,
            message.Data);

        return Task.CompletedTask;
    }
}
