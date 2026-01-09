using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Seeder;
using JetBrains.Annotations;

namespace Inventory_Backend_NET.TestOnlyEndpoint;

internal static class MapTestEndpointExtension
{
    /// <summary>
    /// Mapping API Endpoint khusus untuk E2E testing dengan frontend
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static WebApplication MapTestEndpoints(this WebApplication app)
    {
        var testApiGroup = app.MapGroup("e2e-test-api");
        testApiGroup.MapPost("db/refresh", async (MyDbContext db) =>
        {
            await db.RefreshDatabaseAsync();

            return Results.Ok();
        });
        
        testApiGroup.MapPost("user", async (MyDbContext db, CreateUserRequest request) =>
        {
            await db.Users.AddAsync(new User(
                username: request.Username,
                password: request.Password,
                isAdmin: request.IsAdmin));
            await db.SaveChangesAsync();

            return Results.Ok();
        });
        
        return app;
    }
}

file record CreateUserRequest(
    [property: JsonPropertyName("username")][UsedImplicitly] string Username,
    [property: JsonPropertyName("password")][UsedImplicitly] string Password,
    [property: JsonPropertyName("is_admin")][UsedImplicitly] bool IsAdmin);