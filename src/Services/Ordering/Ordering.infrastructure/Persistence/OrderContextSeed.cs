using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        if (await orderContext.Orders.AnyAsync())
            return;

        orderContext.Orders.AddRange(GetPreconfiguredOrders());
        await orderContext.SaveChangesAsync();
        logger.LogInformation("Seed database associated with context {DbContextName}", nameof(OrderContext));
    }

    private static IEnumerable<Order> GetPreconfiguredOrders()
    {
        return new List<Order>
        {
            new Order
            {
                UserName = "swn",
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@example.com",
                AddressLine = "123 Main St",
                Country = "USA",
                State = "CA",
                ZipCode = "90001",
                CardName = "John Doe",
                CardNumber = "4111111111111111",
                Expiration = "12/26",
                CVV = "123",
                PaymentMethod = 1,
                TotalPrice = 350
            }
        };
    }
}
