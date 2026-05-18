using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Commerce.Infrastructure.Services;

internal class StripePaymentService(IOptions<StripeOptions> stripeOptions) : IPaymentService
{
    public async Task<string> CreatePaymentUrl(Guid paymentId, Guid userId, Guid courseId, decimal amount)
    {
        var optionsValue = stripeOptions.Value;

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            Metadata = new()
            {
                ["paymentId"] = paymentId.ToString(),
                ["courseId"] = courseId.ToString(),
                ["userId"] = userId.ToString()
            },
            LineItems = new()
            {
                new SessionLineItemOptions
                {
                    PriceData = new()
                    {
                        Currency = optionsValue.Currency,
                        UnitAmount = Convert.ToInt64(decimal.Round(amount * 100, 0)),
                        ProductData = new()
                        {
                            Name = "Course enrollment"
                        }
                    },
                    Quantity = 1
                }
            },
            SuccessUrl = optionsValue.SuccessUrl,
            CancelUrl = optionsValue.CancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }
}
