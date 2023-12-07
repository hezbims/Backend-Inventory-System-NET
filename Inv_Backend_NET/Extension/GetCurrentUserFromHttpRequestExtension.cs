using System.Security.Claims;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.Utils;

public static class GetCurrentUserFromHttpRequestExtension
{
    public static User GetCurrentUserFrom(
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
        var user = db.Users.Single(user => user.Username == username);
        return user;
    }
}