using System.Text.Json.Serialization;

namespace Inventory_Backend.Tests.Fitur.Pengaju._Dto;

/// <summary>
/// Merepresentasikan <c>Pengaju</c>
/// </summary>
public record GetStakeholdersResponseTestDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("is_supplier")]
    bool IsSupplier)
{ }