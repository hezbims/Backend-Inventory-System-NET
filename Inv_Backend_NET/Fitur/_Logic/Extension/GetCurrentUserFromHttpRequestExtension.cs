using System.Security.Claims;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class GetCurrentUserFromHttpRequestExtension
{
    public static User? GetCurrentUserFrom(
        this MyDbContext db,
        IHttpContextAccessor httpContextAccessor
    )
    {
        var username = httpContextAccessor
            .HttpContext!
            .User
            .FindFirstValue(
                ClaimTypes.NameIdentifier
            );
        var user = db.Users.FirstOrDefault(user => user.Username == username);
        return user;
    }
    
    public static async Task<User?> GetCurrentUserFromAsync(
        this MyDbContext db,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default
    )
    {
        var username = httpContextAccessor
            .HttpContext!
            .User
            .FindFirstValue(
                ClaimTypes.NameIdentifier
            );
        var user = await db.Users.FirstOrDefaultAsync(
            user => user.Username == username,
            cancellationToken: cancellationToken);
        return user;
    }
}