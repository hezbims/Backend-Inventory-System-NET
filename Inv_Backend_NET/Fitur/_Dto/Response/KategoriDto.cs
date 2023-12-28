using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur._Dto.Response;

public class KategoriDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nama")]
    public string Nama { get; set; } = null!;

    public static KategoriDto From(Database.Models.Kategori kategori)
    {
        return new KategoriDto()
        {
            Id = kategori.Id,
            Nama = kategori.Nama
        };
    }
}