using Microsoft.EntityFrameworkCore;
using Optimize.Host.Entities;

namespace Optimize.Host;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            await Seeding.SeedData(context, cancellationToken);
        }); 
    }
}

public static class Seeding
{
    public static async Task SeedData(DbContext context, CancellationToken cancellationToken)
    {
        if (!context.Set<Order>().Any())
        {
            for(var i = 1; i <= 1000; i++)
            {
                context.Set<Order>().Add(new Order
                {
                    Name = $"Order {i}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}