using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StepLearning.Shared;

namespace Courses.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCoursesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>)
            );
        return services;
    }
}
