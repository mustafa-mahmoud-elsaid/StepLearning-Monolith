namespace Notifications.Application.Models;

public sealed record EmailMessage(
    string To,
    string Subject,
    string Body);
