using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory_Backend.Tests.Helper;

public static class GetJwtHelper
{
    public static string GetJwt(
        this IServiceScope scope,
        bool isAdmin
    )
    {
        var db = scope.ServiceProvider.GetRequiredService <MyDbContext>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

        return jwtService.GenerateNewToken(
            db.Users.First(user => user.IsAdmin == isAdmin)    
        );
    }
}