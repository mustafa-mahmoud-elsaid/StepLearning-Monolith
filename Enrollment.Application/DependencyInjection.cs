using Enrollment.Application.Data;
using Enrollment.Application.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StepLearning.Shared;

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

        services.AddDbContext<EnrollmentDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StepLearning.EnrollmentsDb"),
                sql =>
                {
                    sql.EnableRetryOnFailure(2);
                });
        });
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

        return services;
    }
}
