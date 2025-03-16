using System.Text.Json.Serialization;

namespace Inventory_Backend.Tests.Fitur.User._Dto;

public record GetUserResponseTestDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("username")]
    string Username,
    [property: JsonPropertyName("is_admin")]
    bool IsAdmin)
{
}