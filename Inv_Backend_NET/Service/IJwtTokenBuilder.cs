using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.Service;

public interface IJwtTokenBuilder
{
    string GenerateToken(User user);
}