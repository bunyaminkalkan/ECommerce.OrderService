using ECommerce.OrderService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.OrderService.Infrastructure.Context.PostgreSQL;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory());

        // config dosyasını al
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        // connection string'i çek
        var connectionString = configuration.GetConnectionString("PostgreSQL");

        // context options oluştur
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var domainEventDispatcher = new NoOpDomainEventDispatcher();

        return new AppDbContext(optionsBuilder.Options, domainEventDispatcher);
    }
}
