using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Fitur._Logic.Services;

public interface IJwtTokenService
{
    string GenerateNewToken(User user);
    string GetUsernameFromJwt(string jwt);
}