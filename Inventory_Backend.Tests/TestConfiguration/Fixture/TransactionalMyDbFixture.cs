using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory_Backend.Tests.TestConfiguration.Fixture;

public class TransactionalMyDbFixture
{
    public TransactionalMyDbFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        Cleanup();
        context.SeedStatusPengajuan();
    }

    public void Cleanup()
    {
        using var context = CreateContext();
        
        context.RefreshDatabase();
    }
    
    public MyDbContext CreateContext()
    {
        // Build configuration
        var configuration = TestConfig.GetTestConfig();

        // Set up DbContext with the test connection string
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(configuration.GetConnectionString(MyConstants.AppSettingsKey.MyConnectionString))
            .Options;

        return new MyDbContext(options);
    }
}