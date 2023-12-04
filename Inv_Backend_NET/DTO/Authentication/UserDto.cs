using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.DTO.Authentication;

public class UserDto
{
    [JsonPropertyName("id")] public int Id;
    [JsonPropertyName("username")] public string Username;
    
    [JsonPropertyName("token")] 
    public string? Token { get; set; } = null;
    
    public UserDto(int id, string username)
    {
        Id = id;
        Username = username;
    }

    public static UserDto From(User user)
    {
        return new UserDto(
            id: user.Id,
            username: user.Username
        );
    }
}