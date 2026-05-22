using Enrollment.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StepLearning.Shared;
using StepLearning.Shared.Abstraction;

namespace Enrollment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddEnrollmentApplication(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }
}
