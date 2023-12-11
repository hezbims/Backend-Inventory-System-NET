using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Seeder;

public static class RefreshDatabaseExtension
{
    static string[] _ignoredTables =
    {
        "StatusPengajuans"
    };
    
    public static void RefreshDatabase(this MyDbContext db)
    {
        var allTables = db.Model.GetEntityTypes().Select(e => e.GetTableName()).ToList();
        
        foreach (var table in allTables)
        {
            if (!_ignoredTables.Contains(table))
            {
                db.Database.ExecuteSqlRaw($"DELETE FROM {table}");
            }
        }
    }
}