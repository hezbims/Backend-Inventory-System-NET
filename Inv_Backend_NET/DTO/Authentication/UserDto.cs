using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.DTO.Authentication;

public class UserDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    
    [JsonPropertyName("is_admin")] public bool IsAdmin { get; set; }
    
    public UserDto(int id, string username, bool isAdmin)
    {
        Id = id;
        Username = username;
        IsAdmin = isAdmin;
    }

    public static UserDto From(User user)
    {
        return new UserDto(
            id: user.Id,
            username: user.Username,
            isAdmin: user.IsAdmin
        );
    }
}