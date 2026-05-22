using Enrollment.Application.Repositories;
using Enrollment.Infrastructure.Data;
using Enrollment.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enrollment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEnrollmentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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
