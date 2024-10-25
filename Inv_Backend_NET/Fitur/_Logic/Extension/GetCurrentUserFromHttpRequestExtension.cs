using System.Security.Claims;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

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
}