using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory_Backend.Tests;

public class MyDbFixture : IDisposable
{
    public MyDbContext Db { get; }

    public MyDbFixture()
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        // Set up DbContext with the test connection string
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(configuration.GetConnectionString("YourConnectionStringName"))
            .Options;

        Db = new MyDbContext(options);
    }

    public void Dispose()
    {
        Db.Dispose();
    }
}