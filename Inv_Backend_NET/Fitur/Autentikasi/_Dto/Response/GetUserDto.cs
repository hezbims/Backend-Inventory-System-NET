using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;

public class GetUserDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    
    [JsonPropertyName("is_admin")] public bool IsAdmin { get; set; }
    
    public GetUserDto(int id, string username, bool isAdmin)
    {
        Id = id;
        Username = username;
        IsAdmin = isAdmin;
    }

    public static GetUserDto From(User user)
    {
        return new GetUserDto(
            id: user.Id,
            username: user.Username,
            isAdmin: user.IsAdmin
        );
    }
}