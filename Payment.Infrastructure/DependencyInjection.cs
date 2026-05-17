using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Repositories;
using Payment.Application.ServicesInterfaces;
using Payment.Infrastructure.Data;
using Payment.Infrastructure.Options;
using Payment.Infrastructure.Repositories;
using Payment.Infrastructure.Services;
using Stripe;

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
        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        services.AddScoped<IPaymentService, StripePaymentService>();

        return services;
    }
}
