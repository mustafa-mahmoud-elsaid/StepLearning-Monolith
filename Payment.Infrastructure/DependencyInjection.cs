using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Repositories;
using Payment.Infrastructure.Data;
using Payment.Infrastructure.Repositories;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StepLearning.PaymentsDb"), sql =>
            {
                sql.EnableRetryOnFailure(2);
            });
        });

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
