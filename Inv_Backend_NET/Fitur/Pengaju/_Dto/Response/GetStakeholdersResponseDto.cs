using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur.Pengaju._Dto.Response;

/// <summary>
/// Merepresentasikan <c>Pengaju</c>
/// </summary>
public record GetStakeholdersResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("is_supplier")]
    bool IsSupplier)
{ }