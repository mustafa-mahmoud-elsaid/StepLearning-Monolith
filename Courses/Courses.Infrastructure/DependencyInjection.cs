using Courses.Application.RepositoriesContracts;
using Courses.Infrastructure.Data;
using Courses.Infrastructure.Repositories;
using Courses.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StepLearning.Shared.Abstraction;

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
        services.AddScoped<ICoursesRepository, CoursesRepository>();
        services.AddScoped<ISectionsRepository, SectionsRepository>();
        services.AddScoped<ICourseService, CourseService>();
        return services;
    }
}
