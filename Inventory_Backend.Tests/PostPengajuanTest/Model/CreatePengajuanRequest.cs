using System.Text.Json.Serialization;

namespace Inventory_Backend.Tests.PostPengajuanTest.Model;

public record CreatePengajuanRequest
{
    [JsonPropertyName("id")] 
    public int? IdPengajuan { get; init; } = null;
    
    [JsonPropertyName("id_pengaju")]
    public required int IdPegaju { get; init; }
    
    [JsonPropertyName("list_barang_ajuan")]
    public required List<BarangAjuanRequest> ListBarangAjuan { get; init; } 
}

public record BarangAjuanRequest
{
    [JsonPropertyName("quantity")]
    public required int Quantity { get; init; }
    
    [JsonPropertyName("keterangan")]
    public required string Keterangan { get; init; }
    
    [JsonPropertyName("id_barang")]
    public required int IdBarang { get; init; }
}