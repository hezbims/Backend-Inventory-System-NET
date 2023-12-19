using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.Service;

public interface IJwtTokenService
{
    string GenerateNewToken(User user);
    string GetUsernameFromJwt(string jwt);
}