using Dapper;
using Npgsql;
using Optimize.Host.Entities;

namespace Optimize.Host.Hosts;

public class OrderBackgroundService(IConfiguration configuration) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Processing orders...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);   
            }
        }, stoppingToken);
    }

    private async Task ProcessOrdersAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("Default"));
        await connection.OpenAsync(cancellationToken);

        var sql = @"
            UPDATE ""Orders""
            SET ""ProcessedAt"" = NOW(), ""UpdatedAt"" = NOW()
            WHERE ""Id"" IN (
                SELECT ""Id"" 
                FROM ""Orders""
                WHERE ""ProcessedAt"" IS NULL
                ORDER BY ""CreatedAt""
                LIMIT 10
                FOR UPDATE SKIP LOCKED
            )
            RETURNING ""Id"", ""Name"", ""CreatedAt"", ""UpdatedAt"", ""ProcessedAt"";
        ";
        
        var ordersToProcess = await connection.QueryAsync<Order>(sql, cancellationToken);
        
        foreach (var order in ordersToProcess)
        {
            Console.WriteLine($"Processing order: {order.Id} - {order.Name}");
            // Здесь какая-то тяжелая полезная нагрузка (например, отправка в другой сервис)
        }

    }
}