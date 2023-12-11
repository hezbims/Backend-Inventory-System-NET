using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory_Backend.Tests.TestConfiguration.Fixture;

public class MyDbFixture
{
    private bool _isDatabaseInitialized;
    private static readonly object Lock = new();

    public MyDbFixture()
    {
        lock (Lock)
        {
            if (!_isDatabaseInitialized)
            {
                using (var db = CreateContext())
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    db.SeedStatusPengajuan();
                }
                
                _isDatabaseInitialized = true;
            }
        }
    }

    public MyDbContext CreateContext()
    {
        // Build configuration
        var configuration = TestConfig.GetTestConfig();

        // Set up DbContext with the test connection string
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(configuration.GetConnectionString(MyConstants.AppSettingsKey.MyConnectionString))
            .Options;

        var db = new MyDbContext(options);
        db.Database.BeginTransaction();
        return db;
    }
}