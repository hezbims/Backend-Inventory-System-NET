using Inventory_Backend_NET.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inventory_Backend_NET.Seeder;

public static class RefreshDatabaseExtension
{
    /// <summary>
    /// Deleta all data from all tables
    /// </summary>
    public static void RefreshDatabase(this MyDbContext db)
    {
        var tableNames = TopologicalSortEntities(db)
            .Select(entity => entity.GetTableName()!);
        
        foreach (var table in tableNames)
        {
            var query = $"DELETE FROM {table}";
            db.Database.ExecuteSqlRaw(query);
        }
        
        db.SaveChanges();
    }

    /// <summary>
    /// Handling foreign key constraint that restrict deletion
    /// </summary>
    private static List<IEntityType> TopologicalSortEntities(MyDbContext db)
    {
        var sortedEntities = new List<IEntityType>();
        var visited = new HashSet<IEntityType>();
        
        void Visit(IEntityType entity)
        {
            if (!visited.Add(entity))
                return;
            
            var foreignKeyEntities = entity
                .GetReferencingForeignKeys()
                .Select(foreignKey => foreignKey.DeclaringEntityType);
            
            foreach (IEntityType foreignKeyEntity in foreignKeyEntities)
                Visit(foreignKeyEntity);
            
            sortedEntities.Add(entity);
        }

        foreach (IEntityType entityTarget in db.Model.GetEntityTypes())
            Visit(entityTarget);

        return sortedEntities;
    }
}