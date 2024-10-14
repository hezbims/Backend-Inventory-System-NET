using Inventory_Backend_NET.Database;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Startup;

public static class MigrateDatabase
{
    public static WebApplication MigrateDatabases(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<MyDbContext>()!;
        context.Database.Migrate();

        return app;
    }
}