using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory_Backend.Tests.TestConfiguration;

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
                }
                _isDatabaseInitialized = true;
            }
        }
    }

    public MyDbContext CreateContext()
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        // Set up DbContext with the test connection string
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(configuration.GetConnectionString(MyConstants.AppSettingsKey.MyConnectionString))
            .Options;

        return new MyDbContext(options);
    }
}