using Identity.Application.Interfaces;
using Identity.Application.RepositoryInterfaces;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.JWT;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StepLearning.Shared.Abstraction;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StepLearning.UsersDb"),
                sql => 
                    sql.EnableRetryOnFailure(2)
                    );
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IInstructorService, InstructorService>();
        services.AddScoped<IStudentService, StudentService>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>().AddEntityFrameworkStores<UsersDbContext>();
        return services;
    }
}
