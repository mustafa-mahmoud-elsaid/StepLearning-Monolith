using Commerce.Domain.Orders;
using Commerce.Application.Payment.ServicesInterfaces;
using Commerce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Commerce.Infrastructure.Services;

internal class StripePaymentService(IOptions<StripeOptions> stripeOptions) : IPaymentService
{
    public async Task<string> CreatePaymentUrl(Guid paymentId, Guid userId, Order order)
    {
        var optionsValue = stripeOptions.Value;

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            Metadata = new()
            {
                ["paymentId"] = paymentId.ToString(),
                ["orderId"] = order.Id.ToString(),
                ["userId"] = userId.ToString()
            },
            LineItems = order.Items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = optionsValue.Currency,
                    UnitAmount = Convert.ToInt64(decimal.Round(item.Price * 100, 0)),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.CourseTitle
                    }
                },
                Quantity = 1
            }).ToList(),
            SuccessUrl = optionsValue.SuccessUrl,
            CancelUrl = optionsValue.CancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }
}
