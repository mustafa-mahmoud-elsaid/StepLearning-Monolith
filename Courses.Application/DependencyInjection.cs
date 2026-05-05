using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

internal sealed class MediatrAssembly { }
public static class DependencyInjection
{
    public static IServiceCollection AddCoursesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(MediatrAssembly).Assembly);
        });
        return services;
    }
}
