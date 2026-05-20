using Commerce.Application.Cart.Repositories;
using Commerce.Application.Orders.Repositories;
using Commerce.Application.Payment.Repositories;
using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Infrastructure.Data;
using Commerce.Infrastructure.Options;
using Commerce.Infrastructure.Repositories;
using Commerce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace Commerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommerceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommerceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("StepLearning.CommerceDb"), sql =>
            {
                sql.EnableRetryOnFailure(2);
            });
        });

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddScoped<IPaymentWebhookService, StripePaymentWebhookService>();

        return services;
    }
}
