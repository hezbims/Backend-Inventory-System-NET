using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inventory_Backend_NET.Seeder;

public static class RefreshDatabaseExtension
{
    
    
    public static void RefreshDatabase(this MyDbContext db)
    {
        IEntityType[] ignoredEntities =
        {
            db.StatusPengajuans.EntityType
        };
        
        var allEntities = db.Model.GetEntityTypes();
        var filteredTables = allEntities.Where(
            entity => !ignoredEntities.Contains(entity)
        ).Select(entity => entity.GetTableName());

        foreach (var table in filteredTables)
        {
            var query = $"DELETE FROM {table}";
            db.Database.ExecuteSqlRaw(query);
        }
        
        db.SaveChanges();
    }
}