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
    public static IServiceCollection AddIdentityApplictaion(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
