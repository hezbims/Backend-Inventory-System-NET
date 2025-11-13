using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Seeder;

namespace Inventory_Backend_NET.Startup;

public static class PrepareSeederHandler
{
    /// <summary>
    /// Menjalankan seeder berdasarkan arguments dari CLI
    /// </summary>
    /// <returns>Mengembalikan nilai boolean apakah ada seeder yang berhasil dijalankan</returns>
    public static bool HandleSeedingCommandFromCli(
        this WebApplication app,
        string[] args
    )
    {
        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Local")) return false;
        
        var containsSeederKeyword = false;
        using var scope = app.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<MyDbContext>();

        if (args.Contains("refresh"))
        {
            db.RefreshDatabase();
            containsSeederKeyword = true;
        }

        if (args.Contains("user-only"))
        {
            new ThreeUserSeeder(db: db).Run();
            containsSeederKeyword = true;
        }
        else if (args.Contains("test-seeder"))
        {
            new CompleteSeeder(serviceProvider: services, cliArgs: args).Run();
            containsSeederKeyword = true;
        }
        

        return containsSeederKeyword;
    }
}