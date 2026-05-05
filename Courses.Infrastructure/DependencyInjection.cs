using Courses.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoursesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoursesDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("CoursesDbConnection"), sql =>
            {
                sql.EnableRetryOnFailure(2);
            });
        });

        return services;
    }
}
