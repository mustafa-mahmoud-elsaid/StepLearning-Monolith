using FluentValidation;
using Identity.Application.Behaviors;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.Data;
using Identity.Application.Infrastructure.JWT;
using Identity.Application.Infrastructure.Repositories;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Identity.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Add Identity Module Services Of eLearning application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StepLearning.UsersDb"),
                sql => 
                    sql.EnableRetryOnFailure(2)
                    );
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>)
            );

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>().AddEntityFrameworkStores<UsersDbContext>();
        return services;
    }
}
