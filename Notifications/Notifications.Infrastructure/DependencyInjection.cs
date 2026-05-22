using Microsoft.Extensions.DependencyInjection;
using Notifications.Application;
using Notifications.Application.Abstractions;
using Notifications.Infrastructure.Services;

namespace Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
    {
        services.AddNotificationsApplication();
        services.AddScoped<IEmailSender, LoggingEmailSender>();

        return services;
    }
}
